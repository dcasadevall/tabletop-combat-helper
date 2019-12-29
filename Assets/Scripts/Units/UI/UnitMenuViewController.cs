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
                              IUnitActionPlanner unitActionPlanner,
                              IGridPositionCalculator gridPositionCalculator,
                              IGridUnitManager gridUnitManager,
                              IInputLock inputLock,
                              ILogger logger) {
            _camera = worldCamera;
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

        // Maybe we should have a different handler to do this logic?
        private async void HandleMoveUnitButtonPressed() {
            // Hide the top menu. This releases input lock, so reacquire it.
            Hide();
            var lockId = _inputLock.Lock();
            if (lockId == null) {
                _logger.LogError(LoggedFeature.Units, "Could not acquire input lock");
                return;
            }

            // Action Planning / Confirmation
            _logger.Log(LoggedFeature.Units, "Planning Action: SelectMoveDestination");
            var destinationResult = await _unitActionPlanner.PlanAction(_unit, UnitAction.SelectMoveDestination);
            _logger.Log(LoggedFeature.Units, "Done Planning Action: SelectMoveDestination");
            if (destinationResult.resultType == UnitActionPlanResult.PlanResultType.Canceled) {
                await UniTask.DelayFrame(5);
                _inputLock.Unlock(lockId.Value);
                return;
            }
            
            _logger.Log(LoggedFeature.Units, "Planning Action: ChooseMovePath");
            var choosePathResult = await _unitActionPlanner.PlanAction(_unit, UnitAction.ChooseMovePath);
            _logger.Log(LoggedFeature.Units, "Done Planning Action: ChooseMovePath");
            if (choosePathResult.resultType == UnitActionPlanResult.PlanResultType.Canceled) {
                await UniTask.DelayFrame(5);
                _inputLock.Unlock(lockId.Value);
                return;
            }
            
            _logger.Log(LoggedFeature.Units, "Planning Action: AnimateMovement");
            await _unitActionPlanner.PlanAction(_unit, UnitAction.AnimateMovement);
            _logger.Log(LoggedFeature.Units, "Done Planning Action: AnimateMovement");
            
            // Release input lock delay to avoid input conflicts
            await UniTask.DelayFrame(5);
            _inputLock.Unlock(lockId.Value);
        }
        
        private void HandleCancelButtonPressed() {
            Hide();
        }
    }
}