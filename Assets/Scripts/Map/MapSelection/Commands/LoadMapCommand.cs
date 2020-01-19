using System;
using System.Collections.Generic;
using CommandSystem;
using Logging;
using Map.MapSections.Commands;
using Map.Serialized;
using Replays.Persistence.UI;
using UI;
using UniRx;
using UnityEngine.SceneManagement;
using Zenject;

namespace Map.MapSelection.Commands {
    public class LoadMapCommand : ICommand {
        private readonly LoadMapCommandData _data;
        private readonly List<IMapReference> _mapReferences;
        private readonly ICommandFactory _commandFactory;
        private readonly ILogger _logger;
        private readonly IReplayLoaderViewController _replayLoaderViewController;
        private readonly IMapSelectViewController _mapSelectViewController;
        private readonly ZenjectSceneLoader _sceneLoader;
        private readonly IModalViewController _modalViewController;
        private readonly Subject<Unit> _sceneLoadedSubject = new Subject<Unit>();

        public bool IsInitialGameStateCommand {
            get {
                return true;
            }
        }

        public LoadMapCommand(LoadMapCommandData data,
                              List<IMapReference> mapReferences,
                              ICommandFactory commandFactory,
                              ILogger logger,
                              ZenjectSceneLoader sceneLoader,
                              IModalViewController modalViewController) {
            _data = data;
            _mapReferences = mapReferences;
            _commandFactory = commandFactory;
            _logger = logger;
            _sceneLoader = sceneLoader;
            _modalViewController = modalViewController;
        }

        public IObservable<Unit> Run() {
            if (_data.mapIndex >= _mapReferences.Count) {
                string errorMsg = string.Format("Invalid map index: {0}", _data.mapIndex);
                _logger.LogError(LoggedFeature.Map, errorMsg);
                return Observable.Throw<Unit>(new Exception(errorMsg));
            }

            _modalViewController.Show("Loading Assets...");
            IMapReference mapReference = _mapReferences[(int) _data.mapIndex];
            // TODO: Commands to use unitask. this should just be all async / await
            IObservable<MapData> mapDataObservable = mapReference.LoadMap().ToObservable();
            mapDataObservable.Subscribe(mapData => {
                _sceneLoader.LoadSceneAsync(_data.SceneName,
                                            LoadSceneMode.Additive,
                                            container => {
                                                HandleMapSceneLoaded(container, mapData);
                                            });      
            });

            return _sceneLoadedSubject;
        }

        private void HandleMapSceneLoaded(DiContainer container, MapData mapData) {
            container.Bind<LoadMapCommandData>().FromInstance(_data);
            container.Bind<IMapData>().FromInstance(mapData);
            // MapSection command may inject mutable map data if on editor mode.
            container.Bind<MapData>().FromInstance(mapData).WhenInjectedInto<LoadMapSectionCommand>();

            // This needs to happen after 1 frame because we are currently still loading the next scene.
            // Otherwise, the dependency graph cannot be yet built.
            Observable.NextFrame().Subscribe(onNext => {
                // This needs to be created directly since the section command is dependant on this command.
                LoadMapSectionCommandData loadMapSectionCommandData = new LoadMapSectionCommandData(0, _data);
                ICommand loadMapSectionCommand = _commandFactory.Create(typeof(LoadMapSectionCommand),
                                                                        typeof(LoadMapSectionCommandData),
                                                                        loadMapSectionCommandData);
                loadMapSectionCommand.Run().Subscribe(next => {
                    _modalViewController.Hide();
                    _sceneLoadedSubject.OnNext(Unit.Default);
                });
            });
        }

        public void Undo() {
            // Not supported
        }
    }
}