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

        // This state is of transient scope, so we can preserve state for Undo()
        private IntVector2? _previousCoords;

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
            IUnit unit = _unitRegistry.GetUnit(_data.unitId);
            if (unit == null) {
                string errorMsg = string.Format("Unit not found in registry: {0}", _data.unitId);
                _logger.LogError(LoggedFeature.Units, errorMsg);
                return Observable.Throw<UniRx.Unit>(new Exception(errorMsg));
            }

            _previousCoords = _gridUnitManager.GetUnitCoords(unit);
            _gridUnitManager.PlaceUnitAtTile(unit, _data.tileCoords);

            return Observable.ReturnUnit();
        }

        public void Undo() {
            if (_previousCoords == null) {
                _logger.LogError(LoggedFeature.Units,
                                 "Previous coords not found for MoveUnitCommand.Undo(): {0}",
                                 _data.unitId);
                return;
            }

            IUnit unit = _unitRegistry.GetUnit(_data.unitId);
            _gridUnitManager.PlaceUnitAtTile(unit, _previousCoords.Value);
        }
    }
}