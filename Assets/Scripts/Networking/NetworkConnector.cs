using Zenject;

namespace Networking {
    /// <summary>
    /// Class that will automatically connect to a network session when initialized.
    /// Initialization time is delegated to however the <see cref="IInitializable"/> object is bound via
    /// zenject installers.
    ///
    /// A common use case is for this object to be initialized at <see cref="SceneContext"/> time, whenever
    /// an interim lobby scene is loaded.
    /// </summary>
    public class NetworkConnector : IInitializable {
        private INetworkManager _networkManager;
        
        public NetworkConnector(INetworkManager networkManager) {
            _networkManager = networkManager;
        }
        
        public void Initialize() {
            _networkManager.Connect();
        }
    }
}