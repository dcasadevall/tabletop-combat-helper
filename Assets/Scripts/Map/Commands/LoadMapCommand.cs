using System;
using System.Collections.Generic;
using CommandSystem;
using Grid.Serialized;
using Logging;
using UnityEngine.SceneManagement;
using Zenject;

namespace Map.Commands {
    public class LoadMapCommand : ICommand {
        private readonly LoadMapCommandData _data;
        private readonly List<IMapData> _mapDatas;
        private readonly ILogger _logger;
        private readonly ZenjectSceneLoader _sceneLoader;

        // TODO: Inject this
        private const string kCombatSceneName = "CombatScene";

        public bool IsInitialGameStateCommand {
            get {
                return true;
            }
        }

        public LoadMapCommand(LoadMapCommandData data, List<IMapData> mapDatas, ILogger logger, ZenjectSceneLoader sceneLoader) {
            _data = data;
            _mapDatas = mapDatas;
            _logger = logger;
            _sceneLoader = sceneLoader;
        }

        public void Run() {
            if (_data.mapIndex > _mapDatas.Count) {
                _logger.LogError(LoggedFeature.Map, "Invalid map index: {0}", _data.mapIndex);
                return;
            }
            
            IMapData mapData = _mapDatas[(int)_data.mapIndex];
            
            _sceneLoader.LoadScene(kCombatSceneName , LoadSceneMode.Additive, container => {
                container.Bind<IMapData>().FromInstance(mapData);
                container.Bind<IGridData>().FromInstance(mapData.GridData);
            });
        }

        public void Undo() {
            // Not supported
        }
    }
}