using CommandSystem;
using Grid;
using Grid.Commands;
using Math;
using UnityEngine;

namespace Units.Actions.Listeners.Move {
    public class UnitGridPositionPreviewer : IUnitActionListener {
        private readonly ICommandQueue _commandQueue;
        private readonly IGridUnitManager _gridUnitManager;
        private readonly IGridInputManager _gridInputManager;
        private IntVector2? _previousCoordinates;
        
        public UnitAction ActionType {
            get {
                return UnitAction.Move;
            }
        }

        public UnitGridPositionPreviewer(ICommandQueue commandQueue, 
                                         IGridUnitManager gridUnitManager,
                                         IGridInputManager gridInputManager) {
            _commandQueue = commandQueue;
            _gridUnitManager = gridUnitManager;
            _gridInputManager = gridInputManager;
        }

        public void HandleActionPlanned(IUnit unit) {
            _previousCoordinates = _gridUnitManager.GetUnitCoords(unit);
        }

        public void Tick(IUnit unit) {
            if (Input.GetKeyUp(KeyCode.R)) {
                RotateUnitData rotateUnitData = new RotateUnitData(unit.UnitId, 90);
                _commandQueue.Enqueue<RotateUnitCommand, RotateUnitData>(rotateUnitData, CommandSource.Game);
            }
            
            // Unit not in grid.
            if (_previousCoordinates == null) {
                return;
            }
            
            IntVector2? inputCoordinates = _gridInputManager.GetTileAtMousePosition();
            if (inputCoordinates == null) {
                return;
            }

            // Make sure we only send a move command if necessary (tile hasn't changed)
            if (_previousCoordinates.Value == inputCoordinates.Value) {
                return;
            }
            
            // Enqueue the command
            IntVector2 moveDistance = inputCoordinates.Value - _previousCoordinates.Value;
            MoveUnitData moveUnitData = new MoveUnitData(unit.UnitId, moveDistance);
            _commandQueue.Enqueue<MoveUnitCommand, MoveUnitData>(moveUnitData, CommandSource.Game);
            
            _previousCoordinates = inputCoordinates;
        }

        public void HandleActionConfirmed(IUnit unit, IntVector2 tileCoords) {
            _previousCoordinates = null;
        }

        public void HandleActionCanceled(IUnit unit) {
            _previousCoordinates = null;
        }
    }
}