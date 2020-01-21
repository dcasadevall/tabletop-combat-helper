using System;
using System.Collections.Generic;
using CommandSystem;
using Logging;
using Map.MapData;
using Map.MapData.Store;
using Map.MapSections.Commands;
using Replays.Persistence.UI;
using UI;
using UniRx;
using UnityEngine.SceneManagement;
using Zenject;

namespace Map.MapSelection.Commands {
    public class LoadMapCommand : ICommand {
        private readonly LoadMapCommandData _data;
        private readonly IReadOnlyMapAssetStore _mapStore;
        private readonly ICommandFactory _commandFactory;
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
                              IReadOnlyMapAssetStore mapStore,
                              ICommandFactory commandFactory,
                              ZenjectSceneLoader sceneLoader,
                              IModalViewController modalViewController) {
            _data = data;
            _mapStore = mapStore;
            _commandFactory = commandFactory;
            _sceneLoader = sceneLoader;
            _modalViewController = modalViewController;
        }

        public IObservable<Unit> Run() {
            _modalViewController.Show("Loading Assets...");
            // TODO: Commands to use unitask. this should just be all async / await
            IObservable<IMapAsset> mapDataObservable = _mapStore.LoadMap(new MapStoreId(_data.mapIndex)).ToObservable();
            mapDataObservable.Subscribe(mapAsset => {
                _sceneLoader.LoadSceneAsync(_data.SceneName,
                                            LoadSceneMode.Additive,
                                            container => {
                                                HandleMapSceneLoaded(container, mapAsset.MapData);
                                            });      
            });

            return _sceneLoadedSubject;
        }

        private void HandleMapSceneLoaded(DiContainer container, IMutableMapData mapData) {
            // MapSection command may inject mutable map data if on editor mode.
            container.Bind<IMapData>().FromInstance(mapData);
            container.Bind<IMutableMapData>().FromInstance(mapData).WhenInjectedInto<LoadMapSectionCommand>();
            container.Bind<LoadMapCommandData>().FromInstance(_data);

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