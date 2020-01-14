using System;
using System.Collections.Generic;
using CommandSystem;
using Grid;
using Logging;
using Math;
using UniRx;
using Units.Serialized;

namespace Units.Spawning.Commands {
    public class DespawnUnitCommand : ICommand {
        private readonly DespawnUnitData _data;
        private readonly ICommandFactory _commandFactory;
        private readonly IMutableUnitRegistry _unitRegistry;
        private readonly IGridUnitManager _gridUnitManager;
        private readonly IUnitDataIndexResolver _indexResolver;
        private readonly IUnitPool _unitPool;
        private readonly ILogger _logger;

        public bool IsInitialGameStateCommand {
            get {
                return false;
            }
        }

        // Coordinates at which the unit was before despawned. This allows us to undo the command.
        private IntVector2? _tileCoords;
        private IUnit _unit;

        public DespawnUnitCommand(DespawnUnitData data,
                                  ICommandFactory commandFactory,
                                  IMutableUnitRegistry unitRegistry,
                                  IGridUnitManager gridUnitManager,
                                  IUnitDataIndexResolver indexResolver,
                                  IUnitPool unitPool,
                                  ILogger logger) {
            _data = data;
            _commandFactory = commandFactory;
            _unitRegistry = unitRegistry;
            _gridUnitManager = gridUnitManager;
            _indexResolver = indexResolver;
            _unitPool = unitPool;
            _logger = logger;
        }

        public IObservable<UniRx.Unit> Run() {
            _unit = _unitRegistry.GetUnit(_data.unitId);
            if (_unit == null) {
                string errorMsg = $"Unit not found: {_data.unitId}";
                _logger.LogError(LoggedFeature.Units, errorMsg);
                return Observable.Throw<UniRx.Unit>(new IndexOutOfRangeException(errorMsg));
            }

            _tileCoords = _gridUnitManager.GetUnitCoords(_unit);

            // Only despawn the unit. This does not remove pet units.
            _unitPool.Despawn(_unit.UnitId);
            _gridUnitManager.RemoveUnit(_unit);

            return Observable.ReturnUnit();
        }

        public void Undo() {
            if (_unit == null) {
                _logger.LogError(LoggedFeature.Units, "Cannot undo Despawn command without previously running it.");
                return;
            }

            if (_tileCoords == null) {
                _logger.LogError(LoggedFeature.Units, "Cannot undo Despawn command without tileCoords.");
                return;
            }

            // Spawn the unit at the same coords it was despawned. This does not spawn pet units.
            IUnit unit = _unitPool.Spawn(_unit.UnitId, _unit.UnitData, new IUnit[] { });
            _gridUnitManager.PlaceUnitAtTile(unit, _tileCoords.Value);
        }
    }
}