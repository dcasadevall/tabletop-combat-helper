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

        private EventSystem _eventSystem;
        private Camera _camera;
        private IUnitPickerViewController _unitPickerViewController;
        private ISelectionBox _selectionBox;
        private IGridPositionCalculator _gridPositionCalculator;
        private IGridUnitManager _gridUnitManager;
        private IInputLock _inputLock;
        private ILogger _logger;
        private Guid? _lockId;
        private CancellationTokenSource _batchSelectCancellationTokenSource;

        [Inject]
        public void Construct(EventSystem eventSystem,
                              Camera camera,
                              IUnitPickerViewController unitPickerViewController, 
                              IGridPositionCalculator gridPositionCalculator,
                              IGridUnitManager gridUnitManager,
                              ISelectionBox selectionBox,
                              IInputLock inputLock,
                              ILogger logger) {
            _eventSystem = eventSystem;
            _camera = camera;
            _unitPickerViewController = unitPickerViewController;
            _gridPositionCalculator = gridPositionCalculator;
            _gridUnitManager = gridUnitManager;
            _selectionBox = selectionBox;
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

        private async void HandleBatchSelectPressed() {
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
            _batchSelectCancellationTokenSource = new CancellationTokenSource();

            while (!_batchSelectCancellationTokenSource.IsCancellationRequested) {
                // Wait for a click, not on UI.
                await Observable.EveryUpdate()
                                .Where(_ => Input.GetMouseButton(0))
                                .Where(_ => !_eventSystem.IsPointerOverGameObject())
                                .TakeWhile(_ => !_batchSelectCancellationTokenSource.IsCancellationRequested)
                                .FirstOrDefault();
                
                // Check how many units we selected.
                Rect selectionRect = await _selectionBox.Show();
                Rect worldRect = CameraRectUtils.ViewPortRectToWorldRect(_camera, selectionRect);
                IntVector2[] tilesCoveredByRect = _gridPositionCalculator.GetTilesCoveredByRect(worldRect);
                IUnit[] units = _gridUnitManager.GetUnitsAtTiles(tilesCoveredByRect);

                 // Handle Unit Selection.
                _logger.Log(LoggedFeature.Units, "Selected World Space: {0}", worldRect);
                _logger.Log(LoggedFeature.Units, "Selected {0} Units", units.Length);
                if (units.Length > 0) {
                    HandleNormalCursorPressed();
                }
            }
        }

        public void HandleNormalCursorPressed() {
            if (_lockId != null) {
                _inputLock.Unlock(_lockId.Value);
            }

            _normalCursorButton.SetActive(false);
            _batchSelectButton.SetActive(true);
            _batchSelectCancellationTokenSource?.Cancel();
        }
    }
}