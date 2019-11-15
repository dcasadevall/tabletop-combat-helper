using Logging;
using UI;
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
        private const string kEnocunterSelectionSCene = "EncounterSelectionScene";
        private const string kPlayerSelectionScene = "PlayerSelectionScene";

        private ILogger _logger;
        private INetworkManager _networkManager;
        private readonly IModalViewController _modalViewController;
        private ZenjectSceneLoader _sceneLoader;
        
        public NetworkConnector(INetworkManager networkManager,
                                IModalViewController modalViewController, 
                                ZenjectSceneLoader zenjectSceneLoader, 
                                ILogger logger) {
            _networkManager = networkManager;
            _modalViewController = modalViewController;
            _sceneLoader = zenjectSceneLoader;
            _logger = logger;
        }
        
        public void Initialize() {
            // Infinitely reconnect.
            _networkManager.Disconnected.Subscribe(Observer.Create<Unit>(unit => {
                TryReconnect();
            }));
            
            _networkManager.Connect(allowOfflineMode: true).Subscribe(Observer.Create<NetworkConnectionResult>(result => {
                if (result.isServer) {
                    _sceneLoader.LoadScene(kEnocunterSelectionSCene, LoadSceneMode.Additive);
                } else {
                    _sceneLoader.LoadScene(kPlayerSelectionScene, LoadSceneMode.Additive);
                }
            },
            error => {
                _logger .LogError(LoggedFeature .Network, "Connection error: {0}. Will continue offline.", error);
                _sceneLoader.LoadScene(kEnocunterSelectionSCene, LoadSceneMode.Additive);
            }));
        }

        private void TryReconnect() {
            _modalViewController.Show("Reconnecting...");
            _networkManager.Connect(allowOfflineMode: false).Subscribe(Observer.Create<NetworkConnectionResult>(result => {
                _modalViewController.Hide();
            },
            error => {
                TryReconnect();
            }));
        }
    }
}