using System;
using CommandSystem;
using Logging;
using Math;
using UniRx;
using Units;
using Units.Spawning;

namespace Grid.Commands {
    /// <summary>
    /// Command used to place a unit on the grid, or move it from one tile to another.
    /// </summary>
    public class MoveUnitCommand : ICommand {
        private readonly MoveUnitData _data;
        private readonly IGridUnitManager _gridUnitManager;
        private readonly IUnitRegistry _unitRegistry;
        private readonly ILogger _logger;


        public bool IsInitialGameStateCommand {
            get {
                return false;
            }
        }

        public MoveUnitCommand(MoveUnitData data, IGridUnitManager gridUnitManager, IUnitRegistry unitRegistry,
                               ILogger logger) {
            _data = data;
            _gridUnitManager = gridUnitManager;
            _unitRegistry = unitRegistry;
            _logger = logger;
        }

        public IObservable<UniRx.Unit> Run() {
            return DoMoveUnit(_data.unitId, _data.moveDistance);
        }

        public void Undo() {
            DoMoveUnit(_data.unitId, -_data.moveDistance);
        }

        private IObservable<UniRx.Unit> DoMoveUnit(UnitId unitId, IntVector2 moveDistance) {
            IUnit unit = _unitRegistry.GetUnit(unitId);
            if (unit == null) {
                var errorMsg = $"Unit not found in registry: {unitId}";
                _logger.LogError(LoggedFeature.Units, errorMsg);
                return Observable.Throw<UniRx.Unit>(new Exception(errorMsg));
            }

            IntVector2? previousCoords = _gridUnitManager.GetUnitCoords(unit);
            if (previousCoords == null) {
                var errorMsg = $"Unit position not found: {unitId}";
                _logger.LogError(LoggedFeature.Units, errorMsg);
                return Observable.Throw<UniRx.Unit>(new Exception(errorMsg));
            }
            
            _gridUnitManager.PlaceUnitAtTile(unit, moveDistance + previousCoords.Value);
            return Observable.ReturnUnit(); 
        }
    }
}