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
        private readonly IGridUnitManager _gridUnitManager;
        private readonly UnitBehaviour.Pool _unitBehaviourPool;
        private readonly ILogger _logger;

        public SpawnUnitCommand(IUnitSpawnSettings unitSpawnSettings, IGridUnitManager gridUnitManager,
                                UnitBehaviour.Pool unitBehaviourPool, ILogger logger) {
            _unitSpawnSettings = unitSpawnSettings;
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
            
            IUnit unit = new Unit(unitData);
            foreach (var unitInHierarchy in unit.GetUnitsInHierarchy()) {
                _unitBehaviourPool.Spawn(unitInHierarchy);
                _gridUnitManager.PlaceUnitAtTile(unitInHierarchy, data.tileCoords);
            }
        }
    }
}