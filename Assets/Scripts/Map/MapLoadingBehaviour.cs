using Logging;
using Ninject;
using Ninject.Unity;
using Prototype;
using UnityEngine;
using UnityEngine.Networking;
using ILogger = Logging.ILogger;

namespace Map {
    /// <summary>
    /// This behaviour is used for loading the selected map prefab into the scene.
    /// It also hooks up the region camera with the loaded map region's.
    /// The way we do this will most likely change.
    /// </summary>
    public class MapLoadingBehaviour : DIMono {
        public GameObject mapPrefab;
        public PrototypeCameraController cameraController;
        
        [Inject]
        private ILogger Logger { get; set; }

        private void Start() {
            SpawnMap();
        }
        
        private void SpawnMap() {
            if (mapPrefab == null) {
                Logger.LogError(LoggedFeature.Map, "Map Prefab not assigned.");
                return;
            }
            
            if (cameraController == null) {
                Logger.LogError(LoggedFeature.Map, "Map cameraController not assigned.");
                return;
            }

            GameObject instantiatedMap = Instantiate(mapPrefab);
            cameraController.SetRegionHandler(instantiatedMap.GetComponent<RegionHandler>());
            NetworkServer.Spawn(instantiatedMap);
        }
    }
}
