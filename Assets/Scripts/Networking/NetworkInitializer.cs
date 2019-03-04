using Zenject;

namespace Networking {
    /// <summary>
    /// An initializer object that will automatically connect to a network session when initialized.
    /// Initialization time is delegated to however the <see cref="IInitializable"/> object is bound via
    /// zenject installers.
    ///
    /// A common use case is for this object to be initialized at <see cref="SceneContext"/> time, whenever
    /// an interim lobby scene is loaded.
    /// </summary>
    public class NetworkInitializer : IInitializable {
        private INetworkManager _networkManager;
        
        public NetworkInitializer(INetworkManager networkManager) {
            _networkManager = networkManager;
        }
        
        public void Initialize() {
            _networkManager.Connect();
        }
    }
}