using System.Collections.Generic;
using CommandSystem;
using Grid.Serialized;
using Logging;
using UnityEngine.SceneManagement;
using Zenject;

namespace Map.Commands {
    public class LoadMapCommand : ICommand<LoadMapCommandData> {
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

        public LoadMapCommand(List<IMapData> mapDatas, ILogger logger, ZenjectSceneLoader sceneLoader) {
            _mapDatas = mapDatas;
            _logger = logger;
            _sceneLoader = sceneLoader;
        }

        public void Run(LoadMapCommandData data) {
            if (data.mapIndex > _mapDatas.Count) {
                _logger.LogError(LoggedFeature.Map, "Invalid map index: {0}", data.mapIndex);
                return;
            }
            
            IMapData mapData = _mapDatas[(int)data.mapIndex];
            
            _sceneLoader.LoadScene(kCombatSceneName , LoadSceneMode.Additive, container => {
                container.Bind<IMapData>().FromInstance(mapData);
                container.Bind<IGridData>().FromInstance(mapData.GridData);
            });
        }

        public void Undo(LoadMapCommandData data) {
            // Not supported
        }
    }
}