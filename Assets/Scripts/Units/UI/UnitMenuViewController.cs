using System;
using Grid;
using Grid.Positioning;
using InputSystem;
using Logging;
using UI.RadialMenu;
using UniRx.Async;
using Units.Actions;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ILogger = Logging.ILogger;

namespace Units.UI {
    public class UnitMenuViewController : MonoBehaviour {
        [SerializeField]
        private Button _moveUnitButton;

        [SerializeField]
        private Button _cancelButton;

        private Camera _camera;
        private IUnit _unit;
        private IInputLock _inputLock;
        private IUnitActionPlanner _unitActionPlanner;
        private IGridUnitManager _gridUnitManager;
        private IGridPositionCalculator _gridPositionCalculator;
        private IRadialMenu _radialMenu;
        private ILogger _logger;

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
            
            // Set selected unit and events
            _unit = unit;
            _moveUnitButton.onClick.AddListener(HandleMoveUnitButtonPressed);
            _cancelButton.onClick.AddListener(HandleCancelButtonPressed);

            // Show radial menu
            var worldPosition = _gridPositionCalculator.GetTileCenterWorldPosition(unitCoords.Value);
            return _radialMenu.Show(_camera.WorldToScreenPoint(worldPosition));
        }

        public UniTask Hide() {
            _moveUnitButton.onClick.RemoveListener(HandleMoveUnitButtonPressed);
            _cancelButton.onClick.RemoveListener(HandleCancelButtonPressed);
            return _radialMenu.Hide();
        }

        private void HandleMoveUnitButtonPressed() {
            var lockId = _inputLock.Lock();
            if (lockId == null) {
                _logger.LogError(LoggedFeature.Units, "Could not acquire input lock");
                return;
            }

            Hide();
            MoveUnit(_unit, lockId.Value);
        }

        private void HandleCancelButtonPressed() {
            Hide();
        }

        private async void MoveUnit(IUnit unit, Guid lockId) {
            _logger.Log(LoggedFeature.Units, "Planning Action: Move");
            await _unitActionPlanner.PlanAction(unit, UnitAction.Move);
            _logger.Log(LoggedFeature.Units, "Done Planning Action: Move");

            // Wait a few frames to release the input lock so there are no mouse button conflicts.
            await UniTask.DelayFrame(10);
            _inputLock.Unlock(lockId);
        }
    }
}