using System.Collections.Generic;
using CommandSystem;
using Grid;
using Logging;
using Units.Serialized;

namespace Units.Commands {
    /// <summary>
    /// Command used to spawn a unit and position it in the grid.
    /// We may want to consider decoupling those two actions, but this works for now.
    /// </summary>
    public class SpawnUnitCommand : ICommand<SpawnUnitData> {
        private readonly IUnitSpawnSettings _unitSpawnSettings;
        private readonly IUnitDataIndexResolver _unitDataIndexResolver;
        private readonly IMutableUnitRegistry _unitRegistry;
        private readonly IGridUnitManager _gridUnitManager;
        private readonly UnitBehaviour.Pool _unitBehaviourPool;
        private readonly ILogger _logger;

        public SpawnUnitCommand(IUnitSpawnSettings unitSpawnSettings, 
                                IUnitDataIndexResolver unitDataIndexResolver,
                                IMutableUnitRegistry unitRegistry,
                                IGridUnitManager gridUnitManager,
                                UnitBehaviour.Pool unitBehaviourPool, 
                                ILogger logger) {
            _unitSpawnSettings = unitSpawnSettings;
            _unitDataIndexResolver = unitDataIndexResolver;
            _unitRegistry = unitRegistry;
            _gridUnitManager = gridUnitManager;
            _unitBehaviourPool = unitBehaviourPool;
            _logger = logger;
        }
        
        public void Run(SpawnUnitData data) {
            IUnitData[] unitDatas = _unitSpawnSettings.GetUnits(data.unitCommandData.unitType);
            if (data.unitCommandData.unitIndex >= unitDatas.Length) {
                _logger.LogError(LoggedFeature.Units,
                                 "Unit Index not in unit datas range: {0}",
                                 data.unitCommandData.unitIndex);
                return;
            }

            IUnitData unitData = unitDatas[(int)data.unitCommandData.unitIndex];
            _logger.Log(LoggedFeature.Units, "Spawning: {0}", unitData.Name);
            
            // First, spawn the pets recursively.
            IUnit[] pets = new IUnit[data.unitCommandData.pets.Length];
            for (var i = 0; i < data.unitCommandData.pets.Length; i++) {
                Run(new SpawnUnitData(data.unitCommandData.pets[i], data.tileCoords));
                pets[i] = _unitRegistry.GetUnit(data.unitCommandData.pets[i].unitId);
            }
            
            // Now, spawn the unit itself.
            IUnit unit = new Unit(data.unitCommandData.unitId, unitData, pets);
            _unitRegistry.RegisterUnit(unit);
            _unitBehaviourPool.Spawn(unit);
            _gridUnitManager.PlaceUnitAtTile(unit, data.tileCoords);
        }

        public void Undo(SpawnUnitData data) {
            // First, despawn the pets recursively.
            for (var i = 0; i < data.unitCommandData.pets.Length; i++) {
                Undo(new SpawnUnitData(data.unitCommandData.pets[i], data.tileCoords));
            }
            
            // Now the unit itself.
            IUnit unit = _unitRegistry.GetUnit(data.unitCommandData.unitId);
            _unitRegistry.UnregisterUnit(unit.UnitId);
            _unitBehaviourPool.Despawn(unit.UnitId);
            _gridUnitManager.RemoveUnit(unit);
        }
    }
}