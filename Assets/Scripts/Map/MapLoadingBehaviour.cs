using Logging;
using Ninject;
using Ninject.Unity;
using UnityEngine;
using UnityEngine.Networking;
using ILogger = Logging.ILogger;

namespace Map {
    /// <summary>
    /// This behaviour is used for loading the selected map prefab into the scene.
    /// The way we do this will most likely change
    /// </summary>
    public class MapLoadingBehaviour : DIMono {
        public GameObject mapPrefab;
        
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

            GameObject instantiatedMap = Instantiate(mapPrefab);
            NetworkServer.Spawn(instantiatedMap);
        }
    }
}
