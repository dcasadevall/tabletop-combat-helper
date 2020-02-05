using System;
using InputSystem;
using Logging;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ILogger = Logging.ILogger;

namespace UI {
    public class ModalViewController : MonoBehaviour, IModalViewController {
        [SerializeField]
        private Text _text;
        
        private IInputLock _inputLock;
        private ILogger _logger;
        private bool _isShown;
        private IDisposable _lock;
        
        [Inject]
        public void Construct(IInputLock inputLock, ILogger logger) {
            _inputLock = inputLock;
            _logger = logger;
        }
        
        private void Start() {
            Hide();
        }

        public void Show(String text) {
            _text.text = text;
            _isShown = true;
            gameObject.SetActive(true);
        }

        private void Update() {
            if (_lock == null && _isShown && !_inputLock.IsLocked) {
                _logger.Log(LoggedFeature.Network, "ModalViewController acquiring input lock.");
                _lock = _inputLock.Lock();
            }
        }

        public void Hide() {
            _lock?.Dispose();
            _lock = null;
            
            _isShown = false;
            gameObject.SetActive(false);
        }
    }
}