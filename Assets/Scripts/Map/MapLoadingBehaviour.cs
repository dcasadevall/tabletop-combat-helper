using Logging;
using Networking;
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
    public class MapLoadingBehaviour : MonoBehaviour {
        private ILogger _logger;
        private MapBehaviour.Factory _factory;
        private INetworkManager _networkManager;

        [Inject]
        public void Construct(INetworkManager networkManager, MapBehaviour.Factory factory, ILogger logger) {
            _factory = factory;
        }

        private void Start() {
            SpawnMap();
        }
        
        private void SpawnMap() {
            if (!_networkManager.IsServer) {
                return;
            }
            
            MapBehaviour mapBehaviour = _factory.Create();
            NetworkServer.Spawn(mapBehaviour.gameObject);
        }
    }
}
