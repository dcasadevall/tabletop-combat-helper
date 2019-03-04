using Networking;
using UnityEngine.Networking;
using Zenject;

namespace Map {
    /// <summary>
    /// This behaviour is used for loading the selected map prefab into the scene.
    /// It also hooks up the region camera with the loaded map region's.
    /// The way we do this will most likely change.
    /// </summary>
    public class MapLoader : IInitializable {
        private MapBehaviour.Factory _factory;
        private INetworkManager _networkManager;

        public MapLoader(INetworkManager networkManager, MapBehaviour.Factory factory) {
            _factory = factory;
            _networkManager = networkManager;
        }

        public void Initialize() {
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
