using Logging;
using Prototype;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;
using ILogger = Logging.ILogger;

namespace Map {
    /// <summary>
    /// This behaviour is used for loading the selected map prefab into the scene.
    /// It also hooks up the region camera with the loaded map region's.
    /// The way we do this will most likely change.
    /// </summary>
    public class MapLoadingBehaviour : NetworkBehaviour {
        public GameObject mapPrefab;
        public PrototypeCameraController cameraController;

        private ILogger _logger;

        [Inject]
        public void Construct(ILogger logger) {
            _logger = logger;
        }

        private void Start() {
            SpawnMap();
        }
        
        private void SpawnMap() {
            if (mapPrefab == null) {
                _logger.LogError(LoggedFeature.Map, "Map Prefab not assigned.");
                return;
            }
            
            if (cameraController == null) {
                _logger.LogError(LoggedFeature.Map, "Map cameraController not assigned.");
                return;
            }

            if (!isServer) {
                return;
            }
            
            GameObject instantiatedMap = Instantiate(mapPrefab);
            cameraController.SetRegionHandler(instantiatedMap.GetComponent<RegionHandler>());
            NetworkServer.Spawn(instantiatedMap);
        }
    }
}
