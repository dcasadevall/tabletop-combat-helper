using System;
using Grid;
using InputSystem;
using JetBrains.Annotations;
using Math;
using Units.Actions;
using Units.Spawning;
using UnityEngine;
using Zenject;

namespace Units.UI {
    public class UnitSelectionDetector : ITickable {
        private readonly IInputLock _inputLock;
        private readonly IUnitActionPlanner _unitActionPlanner;
        private readonly IGridInputManager _gridInputManager;
        private readonly IUnitRegistry _unitRegistry;
        private readonly IGridUnitManager _gridUnitManager;

        [CanBeNull]
        private Guid? _lockId;

        public UnitSelectionDetector(IInputLock inputLock,
                                     IUnitActionPlanner unitActionPlanner,
                                     IGridInputManager gridInputManager,
                                     IGridUnitManager gridUnitManager) {
            _inputLock = inputLock;
            _unitActionPlanner = unitActionPlanner;
            _gridInputManager = gridInputManager;
            _gridUnitManager = gridUnitManager;
        }

        public void Tick() {
            if (_inputLock.IsLocked && _lockId == null) {
                return;
            }

            if (Input.GetMouseButtonDown(0)) {
                OnMouseDown();
            }
        }

        private async void OnMouseDown() {
            // Acquire input lock. If we fail to do so, return.
            _lockId = _inputLock.Lock();
            if (_lockId == null) {
                return;
            }

            IntVector2? tileCoords = _gridInputManager.GetTileAtMousePosition();
            if (tileCoords == null) {
                return;
            }

            IUnit[] units = _gridUnitManager.GetUnitsAtTile(tileCoords.Value);
            if (units.Length == 0) {
                return;
            }

            await _unitActionPlanner.PlanAction(units[0], UnitAction.Move);
            _inputLock.Unlock(_lockId.Value);
            _lockId = null;
        }
    }
}