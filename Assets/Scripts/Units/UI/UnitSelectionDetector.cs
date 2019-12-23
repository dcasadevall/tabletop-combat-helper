using Grid;
using InputSystem;
using Math;
using Units.Actions;
using Units.Spawning;
using UnityEngine;
using Zenject;
using ILogger = Logging.ILogger;

namespace Units.UI {
    public class UnitSelectionDetector : ITickable {
        private readonly UnitMenuViewController _unitMenuViewController;
        private readonly IInputLock _inputLock;
        private readonly IGridInputManager _gridInputManager;
        private readonly IUnitRegistry _unitRegistry;
        private readonly IGridUnitManager _gridUnitManager;

        public UnitSelectionDetector(UnitMenuViewController unitMenuViewController,
                                     IInputLock inputLock,
                                     IGridInputManager gridInputManager,
                                     IGridUnitManager gridUnitManager) {
            _unitMenuViewController = unitMenuViewController;
            _inputLock = inputLock;
            _gridInputManager = gridInputManager;
            _gridUnitManager = gridUnitManager;
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

            _unitMenuViewController.Show(units[0]);
        }
    }
}