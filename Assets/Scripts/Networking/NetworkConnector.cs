using Logging;
using UniRx;
using UnityEngine.SceneManagement;
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
        // TODO: Inject this
        private const string kMapSelectionScene = "MapSelectionScene";
        private const string kPlayerSelectionScene = "PlayerSelectionScene";

        private ILogger _logger;
        private INetworkManager _networkManager;
        private ZenjectSceneLoader _sceneLoader;
        
        public NetworkConnector(INetworkManager networkManager, ZenjectSceneLoader zenjectSceneLoader, ILogger logger) {
            _networkManager = networkManager;
            _sceneLoader = zenjectSceneLoader;
            _logger = logger;
        }
        
        public void Initialize() {
            _networkManager.Connect().Subscribe(Observer.Create<NetworkConnectionResult>(result => {
                if (result.isServer) {
                    _sceneLoader.LoadScene(kMapSelectionScene, LoadSceneMode.Additive);
                } else {
                    _sceneLoader.LoadScene(kPlayerSelectionScene, LoadSceneMode.Additive);
                }
            },
            error => {
                _logger .LogError(LoggedFeature .Network, "Connection error: {0}", error);
            }));
        }
    }
}