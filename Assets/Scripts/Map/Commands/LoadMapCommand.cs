using System;
using System.Collections.Generic;
using CommandSystem;
using EncounterSelection;
using Grid.Serialized;
using Logging;
using Map.UI;
using Replays.Persistence.UI;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using ILogger = Logging.ILogger;

namespace Map.Commands {
    public class LoadMapCommand : ICommand {
        private readonly LoadMapCommandData _data;
        private readonly List<IMapData> _mapDatas;
        private readonly ILogger _logger;
        private readonly IReplayLoaderViewController _replayLoaderViewController;
        private readonly IMapSelectViewController _mapSelectViewController;
        private readonly ZenjectSceneLoader _sceneLoader;

        // TODO: Inject this
        private const string kCombatSceneName = "CombatScene";

        public bool IsInitialGameStateCommand {
            get {
                return true;
            }
        }

        public LoadMapCommand(LoadMapCommandData data,
                              List<IMapData> mapDatas, ILogger logger, ZenjectSceneLoader sceneLoader) {
            _data = data;
            _mapDatas = mapDatas;
            _logger = logger;
            _sceneLoader = sceneLoader;
        }

        public IObservable<Unit> Run() {
            if (_data.mapIndex > _mapDatas.Count) {
                string errorMsg = string.Format("Invalid map index: {0}", _data.mapIndex);
                _logger.LogError(LoggedFeature.Map, errorMsg);
                return Observable.Throw<Unit>(new Exception(errorMsg));
            }
            
            IMapData mapData = _mapDatas[(int)_data.mapIndex];
            AsyncOperation asyncOperation = _sceneLoader.LoadSceneAsync(kCombatSceneName , LoadSceneMode.Additive, container => {
                container.Bind<IMapData>().FromInstance(mapData);
                container.Bind<IGridData>().FromInstance(mapData.GridData);
            });

            return asyncOperation.ToUniTask().ToObservable();
        }

        public void Undo() {
            // Not supported
        }
    }
}