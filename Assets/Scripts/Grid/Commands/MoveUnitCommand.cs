using CommandSystem;
using Logging;
using Units;

namespace Grid.Commands {
    /// <summary>
    /// Command used to place a unit on the grid, or move it from one tile to another.
    /// </summary>
    public class MoveUnitCommand : ICommand<MoveUnitData> {
        private readonly IGridUnitManager _gridUnitManager;
        private readonly IUnitRegistry _unitRegistry;
        private readonly ILogger _logger;

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
            
            _gridUnitManager.PlaceUnitAtTile(unit, data.tileCoords);
        }
    }
}