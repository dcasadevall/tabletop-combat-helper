using System;
using System.Threading;
using CameraSystem;
using Grid;
using Grid.Positioning;
using InputSystem;
using Logging;
using Math;
using UI.SelectionBox;
using UniRx;
using UniRx.Async;
using Units.Selection;
using Units.Spawning.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using ILogger = Logging.ILogger;

namespace Units.Editing {
    /// <summary>
    /// This MonoBehaviour controls collapsing / expanding the unit toolbar, which can be used to spawn / batch select
    /// units.
    /// </summary>
    public class UnitToolbarViewController : MonoBehaviour, IInitializable, ITickable, IDisposable {
        [SerializeField]
        private GameObject _batchSelectButton;

        [SerializeField]
        private GameObject _normalCursorButton;

        private IUnitPickerViewController _unitPickerViewController;
        private BatchUnitSelectionDetector _batchUnitSelectionDetector;
        private IInputLock _inputLock;
        private ILogger _logger;
        private Guid? _lockId;
        private IDisposable _selectionObserver;

        [Inject]
        public void Construct(BatchUnitSelectionDetector batchUnitSelectionDetector,
                              IUnitPickerViewController unitPickerViewController,
                              IGridPositionCalculator gridPositionCalculator,
                              IGridUnitManager gridUnitManager,
                              ISelectionBox selectionBox,
                              IInputLock inputLock,
                              ILogger logger) {
            _batchUnitSelectionDetector = batchUnitSelectionDetector;
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
            _selectionObserver?.Dispose();
            _selectionObserver = null;

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

        private void HandleBatchSelectPressed() {
            _lockId = _inputLock.Lock();
            if (_lockId == null) {
                _logger.LogError(LoggedFeature.Units, "Failed to acquire input lock.");
                return;
            }

            // Locking causes this menu to hide, and the way events are fired, we get an event before the ownerId
            // is assigned.
            // TODO: Input.Lock() to return owner so we can verify we own the lock.
            Show();

            _normalCursorButton.SetActive(true);
            _batchSelectButton.SetActive(false);
            _selectionObserver = _batchUnitSelectionDetector
                                 .StartDetecting().Subscribe(Observer.Create<IUnit[]>(units => {
                                     _logger.Log(LoggedFeature.Units, "Selected {0} Units", units.Length);
                                     if (units.Length > 0) {
                                         HandleNormalCursorPressed();
                                     }
                                 }));
        }

        public void HandleNormalCursorPressed() {
            if (_lockId != null) {
                _inputLock.Unlock(_lockId.Value);
            }

            _normalCursorButton.SetActive(false);
            _batchSelectButton.SetActive(true);
            _selectionObserver?.Dispose();
            _selectionObserver = null;
        }
    }
}