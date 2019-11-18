using CommandSystem;
using Grid;
using Grid.Commands;
using Math;
using UnityEngine;

namespace Units.Actions.Listeners.Move {
    public class UnitGridPositionPreviewer : IUnitActionListener {
        private readonly ICommandQueue _commandQueue;
        private readonly IGridInputManager _gridInputManager;
        private IntVector2? _previousCoordinates;
        
        public UnitAction ActionType {
            get {
                return UnitAction.Move;
            }
        }

        public UnitGridPositionPreviewer(ICommandQueue commandQueue, IGridInputManager gridInputManager) {
            _commandQueue = commandQueue;
            _gridInputManager = gridInputManager;
        }

        public void HandleActionPlanned(IUnit unit) {
        }

        public void Tick(IUnit unit) {
            if (Input.GetKeyUp(KeyCode.R)) {
                RotateUnitData rotateUnitData = new RotateUnitData(unit.UnitId, 90);
                _commandQueue.Enqueue<RotateUnitCommand, RotateUnitData>(rotateUnitData, CommandSource.Game);
            }
            
            IntVector2? gridCoordinates = _gridInputManager.GetTileAtMousePosition();
            if (gridCoordinates == null) {
                return;
            }

            // Make sure we only send a move command if necessary (tile hasn't changed)
            if (_previousCoordinates != null && _previousCoordinates.Value == gridCoordinates.Value) {
                return;
            }
            _previousCoordinates = gridCoordinates;
            
            MoveUnitData moveUnitData = new MoveUnitData(unit.UnitId, gridCoordinates.Value);
            _commandQueue.Enqueue<MoveUnitCommand, MoveUnitData>(moveUnitData, CommandSource.Game);
        }

        public void HandleActionConfirmed(IUnit unit, IntVector2 tileCoords) {
            _previousCoordinates = null;
        }

        public void HandleActionCanceled(IUnit unit) {
            _previousCoordinates = null;
        }
    }
}