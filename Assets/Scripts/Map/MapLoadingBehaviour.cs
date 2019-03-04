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
        private ILogger _logger;
        private MapBehaviour.Factory _factory;

        [Inject]
        public void Construct(MapBehaviour.Factory factory, ILogger logger) {
            _factory = factory;
        }

        private void Start() {
            SpawnMap();
        }
        
        private void SpawnMap() {
            if (!isServer) {
                return;
            }
            
            MapBehaviour mapBehaviour = _factory.Create();
            NetworkServer.Spawn(mapBehaviour.gameObject);
        }
    }
}
