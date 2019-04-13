using System;
using CommandSystem;
using Logging;
using Math;
using Units;

namespace Grid.Commands {
    /// <summary>
    /// Command used to place a unit on the grid, or move it from one tile to another.
    /// </summary>
    public class MoveUnitCommand : ICommand<MoveUnitData> {
        private readonly IGridUnitManager _gridUnitManager;
        private readonly IUnitRegistry _unitRegistry;
        private readonly ILogger _logger;
        
        // This state is of transient scope, so we can preserve state for Undo()
        private IntVector2? _previousCoords;
        
        public bool IsInitialGameStateCommand {
            get {
                return false;
            }
        }

        public MoveUnitCommand(IGridUnitManager gridUnitManager, IUnitRegistry unitRegistry, ILogger logger) {
            _gridUnitManager = gridUnitManager;
            _unitRegistry = unitRegistry;
            _logger = logger;
        }
        
        public void Run(MoveUnitData data) {
            IUnit unit = _unitRegistry.GetUnit(data.unitId);
            if (unit == null) {
                _logger.LogError(LoggedFeature.Units, "Unit not found in registry: {0}", data.unitId);
                return;
            }

            _previousCoords = _gridUnitManager.GetUnitCoords(unit);
            _gridUnitManager.PlaceUnitAtTile(unit, data.tileCoords);
        }

        public void Undo(MoveUnitData data) {
            if (_previousCoords == null) {
                _logger.LogError(LoggedFeature.Units, "Previous coords not found for MoveUnitCommand.Undo(): {0}", data.unitId);
                return;
            }
            
            IUnit unit = _unitRegistry.GetUnit(data.unitId);
            _gridUnitManager.PlaceUnitAtTile(unit, _previousCoords.Value);
        }
    }
}