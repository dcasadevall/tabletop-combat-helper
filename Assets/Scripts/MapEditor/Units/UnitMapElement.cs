using CommandSystem;
using Grid;
using Grid.Commands;
using Logging;
using Map.MapData;
using MapEditor.MapElement;
using Math;
using Units;
using Units.Serialized;
using Units.Spawning.Commands;

namespace MapEditor.Units {
    public class UnitMapElement : IMapElement {
        private readonly IUnit _unit;
        private readonly ICommandQueue _commandQueue;

        private IntVector2 _tileCoords;
        private readonly ILogger _logger;
        private readonly IMutableMapSectionData _mapSectionData;
        private readonly IUnitDataIndexResolver _unitDataIndexResolver;

        public UnitMapElement(ICommandQueue commandQueue, 
                              IUnit unit, 
                              IntVector2 tileCoords,
                              ILogger logger,
                              IMutableMapSectionData mapSectionData, 
                              IUnitDataIndexResolver unitDataIndexResolver) {
            _unit = unit;
            _tileCoords = tileCoords;
            _logger = logger;
            _mapSectionData = mapSectionData;
            _unitDataIndexResolver = unitDataIndexResolver;
            _commandQueue = commandQueue;
        }

        public void HandleDrag(IntVector2 tileCoords) {
            uint? unitIndex = _unitDataIndexResolver.ResolveUnitIndex(_unit.UnitData);
            if (unitIndex == null) {
                _logger.LogError(LoggedFeature.Units,
                                 "Error dragging unit with name: {0}. Index not resolved.",
                                 _unit.UnitData.Name);
                return;
            }
            
            var unitDataReference = new UnitDataReference(unitIndex.Value, _unit.UnitData.UnitType);
            _mapSectionData.RemoveInitialUnit(_tileCoords, unitDataReference);
            var tileDistance = tileCoords - _tileCoords;
            _tileCoords = tileCoords;
            _commandQueue.Enqueue<MoveUnitCommand, MoveUnitData>(new MoveUnitData(_unit.UnitId, tileDistance),
                                                                 CommandSource.Game);
            _mapSectionData.AddInitialUnit(_tileCoords, unitDataReference);
        }

        public void Remove() {
            uint? unitIndex = _unitDataIndexResolver.ResolveUnitIndex(_unit.UnitData);
            if (unitIndex == null) {
                _logger.LogError(LoggedFeature.Units,
                                 "Error removing unit with name: {0}. Index not resolved.",
                                 _unit.UnitData.Name);
                return;
            }
            
            var unitDataReference = new UnitDataReference(unitIndex.Value, _unit.UnitData.UnitType);
            _mapSectionData.RemoveInitialUnit(_tileCoords, unitDataReference);
            _commandQueue.Enqueue<DespawnUnitCommand, DespawnUnitData>(new DespawnUnitData(_unit.UnitId),
                                                                       CommandSource.Game);
        }
    }
}