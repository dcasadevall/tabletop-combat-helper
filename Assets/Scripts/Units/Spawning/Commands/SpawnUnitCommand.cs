using System;
using CommandSystem;
using Grid;
using Logging;
using UniRx;
using Units.Serialized;
using UnityEngine;
using ILogger = Logging.ILogger;

namespace Units.Spawning.Commands {
    /// <summary>
    /// Command used to spawn a unit and position it in the grid.
    /// We may want to consider decoupling those two actions, but this works for now.
    /// </summary>
    public class SpawnUnitCommand : ICommand {
        private readonly SpawnUnitData _data;
        private readonly ICommandFactory _commandFactory;
        private readonly IUnitSpawnSettings _unitSpawnSettings;
        private readonly IMutableUnitRegistry _unitRegistry;
        private readonly IGridUnitManager _gridUnitManager;
        private readonly IUnitPool _unitPool;
        private readonly ILogger _logger;

        public bool IsInitialGameStateCommand {
            get {
                return _data.isInitialSpawn;
            }
        }

        public SpawnUnitCommand(SpawnUnitData data,
                                ICommandFactory commandFactory,
                                IUnitSpawnSettings unitSpawnSettings,
                                IMutableUnitRegistry unitRegistry,
                                IGridUnitManager gridUnitManager,
                                IUnitPool unitPool,
                                ILogger logger) {
            _data = data;
            _commandFactory = commandFactory;
            _unitSpawnSettings = unitSpawnSettings;
            _unitRegistry = unitRegistry;
            _gridUnitManager = gridUnitManager;
            _unitPool = unitPool;
            _logger = logger;
        }

        public IObservable<UniRx.Unit> Run() {
            IUnitData[] unitDatas = _unitSpawnSettings.GetUnits(_data.unitCommandData.unitType);
            if (_data.unitCommandData.unitIndex >= unitDatas.Length) {
                string errorMsg = string.Format("Unit Index not in unit datas range: {0}",
                                                _data.unitCommandData.unitIndex);
                _logger.LogError(LoggedFeature.Units, errorMsg);
                return Observable.Throw<UniRx.Unit>(new IndexOutOfRangeException(errorMsg));
            }

            IUnitData unitData = unitDatas[(int) _data.unitCommandData.unitIndex];
            _logger.Log(LoggedFeature.Units, "Spawning: {0}", unitData.Name);

            // First, spawn the pets recursively.
            // We create commands that we execute directly, because
            // we don't want to treat these as standalone commands (they are only ever children of this command)
            IUnit[] pets = new IUnit[_data.unitCommandData.pets.Length];
            for (var i = 0; i < _data.unitCommandData.pets.Length; i++) {
                SpawnUnitData petSpawnUnitData =
                    new SpawnUnitData(_data.unitCommandData.pets[i], _data.tileCoords, _data.isInitialSpawn);
                ICommand petSpawnCommand =
                    _commandFactory.Create(typeof(SpawnUnitCommand), typeof(SpawnUnitData), petSpawnUnitData);
                petSpawnCommand.Run();
                pets[i] = _unitRegistry.GetUnit(_data.unitCommandData.pets[i].unitId);
            }

            // Now, spawn the unit itself.
            Debug.Log("Command hash: " + GetHashCode());
            Debug.Log("Pool hash: " + _unitPool.GetHashCode());
            IUnit unit = _unitPool.Spawn(_data.unitCommandData.unitId, unitData, pets);
            _gridUnitManager.PlaceUnitAtTile(unit, _data.tileCoords);

            return Observable.ReturnUnit();
        }

        public void Undo() {
            // Undo is not supported for player units.
            // We may want to consider splitting this command in two: Spawn initial unit / Spawn unit
            if (_data.unitCommandData.unitType == UnitType.Player) {
                return;
            }

            // Despawn pets first.
            // We need to directly run the command since these are children of the parent spawn unit command,
            // and should never be ran as standalone
            for (var i = 0; i < _data.unitCommandData.pets.Length; i++) {
                ICommand petSpawnCommand =
                    _commandFactory.Create(typeof(SpawnUnitCommand),
                                           typeof(SpawnUnitData),
                                           new SpawnUnitData(_data.unitCommandData.pets[i],
                                                             _data.tileCoords,
                                                             _data.isInitialSpawn));
                petSpawnCommand.Undo();
            }

            // Now the unit itself.
            IUnit unit = _unitRegistry.GetUnit(_data.unitCommandData.unitId);
            _unitPool.Despawn(unit.UnitId);
            _gridUnitManager.RemoveUnit(unit);
        }
    }
}