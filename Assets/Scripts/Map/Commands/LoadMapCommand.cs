using System;
using System.Collections.Generic;
using CommandSystem;
using EncounterSelection;
using Grid.Serialized;
using Logging;
using Map.MapSections.Commands;
using Map.UI;
using Replays.Persistence.UI;
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
        private readonly List<IMapData> _mapDatas;
        private readonly ICommandFactory _commandFactory;
        private readonly ILogger _logger;
        private readonly IReplayLoaderViewController _replayLoaderViewController;
        private readonly IMapSelectViewController _mapSelectViewController;
        private readonly ZenjectSceneLoader _sceneLoader;

        public bool IsInitialGameStateCommand {
            get {
                return true;
            }
        }

        public LoadMapCommand(LoadMapCommandData data,
                              List<IMapData> mapDatas, ICommandFactory commandFactory, ILogger logger,
                              ZenjectSceneLoader sceneLoader) {
            _data = data;
            _mapDatas = mapDatas;
            _commandFactory = commandFactory;
            _logger = logger;
            _sceneLoader = sceneLoader;
        }

        public IObservable<Unit> Run() {
            if (_data.mapIndex > _mapDatas.Count) {
                string errorMsg = string.Format("Invalid map index: {0}", _data.mapIndex);
                _logger.LogError(LoggedFeature.Map, errorMsg);
                return Observable.Throw<Unit>(new Exception(errorMsg));
            }

            IMapData mapData = _mapDatas[(int) _data.mapIndex];

            return _sceneLoader.LoadSceneAsync(kEncounterScene,
                                               LoadSceneMode.Additive,
                                               container => {
                                                   HandleMapSceneLoaded(container, mapData);
                                               }).ToUniTask().ToObservable();
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
                loadMapSectionCommand.Run();
            });
        }

        public void Undo() {
            // Not supported
        }
    }
}