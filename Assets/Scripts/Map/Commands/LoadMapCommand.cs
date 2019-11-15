using System;
using System.Collections.Generic;
using CommandSystem;
using EncounterSelection;
using Grid.Serialized;
using Logging;
using Map.MapSections.Commands;
using Map.UI;
using Replays.Persistence.UI;
using UI;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Zenject;
using ILogger = Logging.ILogger;

namespace Map.Commands {
    public class LoadMapCommand : ICommand {
        private const string kEncounterScene = "EncounterScene";

        private readonly LoadMapCommandData _data;
        private readonly List<IMapReference> _mapPreviews;
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
                              List<IMapReference> mapPreviews, ICommandFactory commandFactory, ILogger logger,
                              ZenjectSceneLoader sceneLoader,
                              IModalViewController modalViewController) {
            _data = data;
            _mapPreviews = mapPreviews;
            _commandFactory = commandFactory;
            _logger = logger;
            _sceneLoader = sceneLoader;
            _modalViewController = modalViewController;
        }

        public IObservable<Unit> Run() {
            if (_data.mapIndex > _mapPreviews.Count) {
                string errorMsg = string.Format("Invalid map index: {0}", _data.mapIndex);
                _logger.LogError(LoggedFeature.Map, errorMsg);
                return Observable.Throw<Unit>(new Exception(errorMsg));
            }

            _modalViewController.Show("Loading Assets...");
            IMapReference mapReference = _mapPreviews[(int) _data.mapIndex];
            IObservable<IMapData> mapDataObservable = mapReference.LoadMap();
            mapDataObservable.Subscribe(mapData => {
                _sceneLoader.LoadSceneAsync(kEncounterScene,
                                            LoadSceneMode.Additive,
                                            container => {
                                                HandleMapSceneLoaded(container, mapData);
                                            });      
            });

            return _sceneLoadedSubject;
        }

        private void HandleMapSceneLoaded(DiContainer container, IMapData mapData) {
            container.Bind<LoadMapCommandData>().FromInstance(_data);
            container.Bind<IMapData>().FromInstance(mapData);

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