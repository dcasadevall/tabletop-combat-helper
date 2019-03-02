using Logging;
using Ninject;
using UnityEngine;
using ILogger = Logging.ILogger;

namespace Map {
    /// <summary>
    /// This behaviour is used for loading the selected map prefab into the scene.
    /// The way we do this will most likely change
    /// </summary>
    public class MapLoadingBehaviour : MonoBehaviour {
        public GameObject mapPrefab;
        
        [Inject]
        private ILogger Logger { get; set; }
        
        private void Awake() {
            if (mapPrefab == null) {
                Logger.LogError(LoggedFeature.Map, "Map Prefab not assigned.");
                return;
            }

            Instantiate(mapPrefab);
        }
    }
}
