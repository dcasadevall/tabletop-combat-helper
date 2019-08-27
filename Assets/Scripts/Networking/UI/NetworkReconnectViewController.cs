using System;
using InputSystem;
using JetBrains.Annotations;
using Logging;
using UnityEngine;
using Zenject;
using ILogger = Logging.ILogger;

namespace Networking.UI {
    public class NetworkReconnectViewController : MonoBehaviour, INetworkReconnectViewController {
        private IInputLock _inputLock;
        private ILogger _logger;
        private bool _isShown;

        [CanBeNull]
        private Guid? _lock;
        
        [Inject]
        public void Construct(IInputLock inputLock, ILogger logger) {
            _inputLock = inputLock;
            _logger = logger;
        }
        
        private void Start() {
            Hide();
        }

        public void Show() {
            _isShown = true;
            gameObject.SetActive(true);
        }

        private void Update() {
            if (_lock == null && _isShown) {
                _logger.Log(LoggedFeature.Network, "Network Reconnect VC acquiring input lock.");
                _lock = _inputLock.Lock();
            }
        }

        public void Hide() {
            _isShown = false;
            if (_lock != null) {
                _inputLock.Unlock(_lock.Value);
            }
            
            gameObject.SetActive(false);
        }
    }
}