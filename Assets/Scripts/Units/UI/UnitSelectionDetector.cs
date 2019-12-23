using System;
using Grid;
using InputSystem;
using Logging;
using Math;
using UI.RadialMenu;
using UniRx.Async;
using Units.Actions;
using Units.Spawning;
using UnityEngine;
using Zenject;
using ILogger = Logging.ILogger;

namespace Units.UI {
    public class UnitSelectionDetector : ITickable {
        private readonly IRadialMenu _radialMenu;
        private readonly IInputLock _inputLock;
        private readonly IUnitActionPlanner _unitActionPlanner;
        private readonly IGridInputManager _gridInputManager;
        private readonly IUnitRegistry _unitRegistry;
        private readonly IGridUnitManager _gridUnitManager;
        private readonly ILogger _logger;

        public UnitSelectionDetector(IRadialMenu radialMenu,
                                     IInputLock inputLock,
                                     IUnitActionPlanner unitActionPlanner,
                                     IGridInputManager gridInputManager,
                                     IGridUnitManager gridUnitManager,
                                     ILogger logger) {
            _radialMenu = radialMenu;
            _inputLock = inputLock;
            _unitActionPlanner = unitActionPlanner;
            _gridInputManager = gridInputManager;
            _gridUnitManager = gridUnitManager;
            _logger = logger;
        }

        public void Tick() {
            if (_inputLock.IsLocked) {
                return;
            }

            if (Input.GetMouseButtonDown(0)) {
                OnMouseDown();
            }
        }

        private void OnMouseDown() {
            IntVector2? tileCoords = _gridInputManager.GetTileAtMousePosition();
            if (tileCoords == null) {
                return;
            }

            IUnit[] units = _gridUnitManager.GetUnitsAtTile(tileCoords.Value);
            if (units.Length == 0) {
                return;
            }

            // Acquire input lock. If we fail to do so, return.
            Guid? lockId = _inputLock.Lock();
            if (lockId == null) {
                return;
            }

            _radialMenu.Show();
            MoveUnit(units[0], lockId.Value);
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