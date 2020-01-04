using System;
using InputSystem;
using Logging;
using Units.Spawning.UI;
using UnityEngine;
using Zenject;
using ILogger = Logging.ILogger;

namespace Units.Editing {
    /// <summary>
    /// This MonoBehaviour controls collapsing / expanding the unit toolbar, which can be used to spawn / batch select
    /// units.
    /// </summary>
    public class UnitToolbarViewController : MonoBehaviour, IInitializable, ITickable, IDisposable {
        [SerializeField]
        private GameObject _expandedUnitEditingToolbar;

        [SerializeField]
        private GameObject _collapsedUnitEditingToolbar;

        private IUnitPickerViewController _unitPickerViewController;
        private IInputLock _inputLock;
        private ILogger _logger;
        private Guid? _lockId;

        [Inject]
        public void Construct(IUnitPickerViewController unitPickerViewController, 
                              IInputLock inputLock,
                              ILogger logger) {
            _unitPickerViewController = unitPickerViewController;
            _inputLock = inputLock;
            _logger = logger;
        }

        public void Initialize() {
            _inputLock.InputLockAcquired += HandleInputLockAcquired;
            _inputLock.InputLockReleased += HandleInputLockReleased;
            Show();
        }
        
        // TODO: This is a pretty janky way of handling input.
        // Maybe create a separate class to handle this?
        // Or disable these shortcuts altogether...
        public void Tick() {
            if (!gameObject.activeInHierarchy) {
                return;
            }
            
            if (Input.GetKeyUp(KeyCode.U)) {
                _unitPickerViewController.Show();
            }
        }

        public void Dispose() {
            _inputLock.InputLockAcquired -= HandleInputLockAcquired;
            _inputLock.InputLockReleased -= HandleInputLockReleased;
            _unitPickerViewController.ViewControllerDismissed -= HandleUnitPickerDismissed;
        }

        private void Show() {
            gameObject.SetActive(true);
        }

        private void Hide() {
            gameObject.SetActive(false);
        }

        private void HandleInputLockAcquired() {
            if (_lockId != null) {
                return;
            }
            
            Hide();
        }

        private void HandleInputLockReleased() {
            Show();
        }

        public void HandleAddUnitsPressed() {
            Hide();
            _unitPickerViewController.ViewControllerDismissed += HandleUnitPickerDismissed;
            _unitPickerViewController.Show();
        }

        private void HandleUnitPickerDismissed() {
            _unitPickerViewController.ViewControllerDismissed -= HandleUnitPickerDismissed;
            Show();
        }

        public void HandleExpandUnitMenuPressed() {
            _lockId = _inputLock.Lock();
            if (_lockId == null) {
                _logger.LogError(LoggedFeature.Units, "Failed to acquire input lock.");
                return;
            }
            // Locking causes this menu to hide, and the way events are fired, we get an event before the ownerId
            // is assigned.
            // TODO: Input.Lock() to return owner so we can verify we own the lock.
            Show();

            _expandedUnitEditingToolbar.SetActive(true);
            _collapsedUnitEditingToolbar.SetActive(false);
        }

        public void HandleCollapseUnitMenuPressed() {
            if (_lockId != null) {
                _inputLock.Unlock(_lockId.Value);
            }

            _expandedUnitEditingToolbar.SetActive(false);
            _collapsedUnitEditingToolbar.SetActive(true);
        }
    }
}