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
        private readonly UnitMenuViewController _unitMenuViewController;
        private readonly IInputLock _inputLock;
        private readonly IUnitActionPlanner _unitActionPlanner;
        private readonly IGridInputManager _gridInputManager;
        private readonly IUnitRegistry _unitRegistry;
        private readonly IGridUnitManager _gridUnitManager;
        private readonly ILogger _logger;

        public UnitSelectionDetector(UnitMenuViewController unitMenuViewController,
                                     IInputLock inputLock,
                                     IUnitActionPlanner unitActionPlanner,
                                     IGridInputManager gridInputManager,
                                     IGridUnitManager gridUnitManager,
                                     ILogger logger) {
            _unitMenuViewController = unitMenuViewController;
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
            
            ShowUnitMenuViewController(units[0], lockId.Value);
        }

        private async void ShowUnitMenuViewController(IUnit unit, Guid lockId) {
            await _unitMenuViewController.Show(unit); 
            
            // Wait a few frames to release the input lock so there are no mouse button conflicts.
            await UniTask.DelayFrame(10);
            _inputLock.Unlock(lockId);
        }
    }
}