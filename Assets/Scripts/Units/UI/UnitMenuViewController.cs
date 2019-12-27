using System;
using System.Threading;
using Grid;
using Grid.Positioning;
using InputSystem;
using Logging;
using UI.RadialMenu;
using UniRx;
using UniRx.Async;
using Units.Actions;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ILogger = Logging.ILogger;

namespace Units.UI {
    /// <summary>
    /// ViewController used for the root HUD shown when a unit is selected.
    /// </summary>
    public class UnitMenuViewController : MonoBehaviour {
        [SerializeField]
        private Button _moveUnitButton;

        [SerializeField]
        private Button _cancelButton;

        private Camera _camera;
        private MoveUnitMenuViewController _moveUnitMenuViewController;
        private Guid? _lockId;
        private IUnit _unit;
        private IInputLock _inputLock;
        private IUnitActionPlanner _unitActionPlanner;
        private IGridUnitManager _gridUnitManager;
        private IGridPositionCalculator _gridPositionCalculator;
        private IRadialMenu _radialMenu;
        private ILogger _logger;
        private Vector3 _menuScreenPositon;

        [Inject]
        public void Construct(Camera worldCamera,
                              MoveUnitMenuViewController moveUnitMenuViewController,
                              IUnitActionPlanner unitActionPlanner,
                              IGridPositionCalculator gridPositionCalculator,
                              IGridUnitManager gridUnitManager,
                              IInputLock inputLock,
                              ILogger logger) {
            _camera = worldCamera;
            _moveUnitMenuViewController = moveUnitMenuViewController;
            _unitActionPlanner = unitActionPlanner;
            _gridPositionCalculator = gridPositionCalculator;
            _gridUnitManager = gridUnitManager;
            _inputLock = inputLock;
            _logger = logger;

            // TODO: Can we inject this instead?
            _radialMenu = GetComponent<IRadialMenu>();
        }

        public UniTask Show(IUnit unit) {
            var unitCoords = _gridUnitManager.GetUnitCoords(unit);
            if (unitCoords == null) {
                var msg = $"Unit not in tile: {unit}";
                _logger.LogError(LoggedFeature.Units, msg);
                return UniTask.FromException(new Exception(msg));
            }

            // Acquire input lock. If we fail to do so, return.
            _lockId = _inputLock.Lock();
            if (_lockId == null) {
                var msg = "Failed to acquire input lock.";
                _logger.LogError(LoggedFeature.Units, msg);
                return UniTask.FromException(new Exception(msg));
            }

            // Set selected unit and events
            _unit = unit;
            _moveUnitButton.onClick.AddListener(HandleMoveUnitButtonPressed);
            _cancelButton.onClick.AddListener(HandleCancelButtonPressed);

            // Show radial menu
            var worldPosition = _gridPositionCalculator.GetTileCenterWorldPosition(unitCoords.Value);
            _menuScreenPositon = _camera.WorldToScreenPoint(worldPosition);
            return _radialMenu.Show(_menuScreenPositon);
        }

        public UniTask Hide() {
            if (_lockId != null) {
                _inputLock.Unlock(_lockId.Value);
            }

            _moveUnitButton.onClick.RemoveListener(HandleMoveUnitButtonPressed);
            _cancelButton.onClick.RemoveListener(HandleCancelButtonPressed);
            return _radialMenu.Hide();
        }

        private async void HandleMoveUnitButtonPressed() {
            // Hide the top menu. This releases input lock, so reacquire it.
            Hide();
            var lockId = _inputLock.Lock();
            if (lockId == null) {
                _logger.LogError(LoggedFeature.Units, "Could not acquire input lock");
                return;
            }

            // Show MoveUnitViewController Submenu.
            var moveUnitMenuTask = _moveUnitMenuViewController.Show(_menuScreenPositon, CancellationToken.None);
            
            // Action Planning / Confirmation
            _logger.Log(LoggedFeature.Units, "Planning Action: Move");
            await _unitActionPlanner.PlanAction(_unit, UnitAction.Move, moveUnitMenuTask.ToObservable());
            _logger.Log(LoggedFeature.Units, "Done Planning Action: Move");

            // Hide Submenu
            await _moveUnitMenuViewController.Hide();
           
            // Release input lock
            _inputLock.Unlock(lockId.Value);
        }
        
        private void HandleCancelButtonPressed() {
            Hide();
        }
    }
}