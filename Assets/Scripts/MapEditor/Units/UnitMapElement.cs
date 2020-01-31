using CommandSystem;
using Grid;
using Grid.Commands;
using Map.MapData;
using MapEditor.MapElement;
using Math;
using Units;
using Units.Spawning.Commands;

namespace MapEditor.Units {
    public class UnitMapElement : IMapElement {
        private readonly IUnit _unit;
        private readonly ICommandQueue _commandQueue;

        private IntVector2 _tileCoords;
        private readonly IMutableMapSectionData _mapSectionData;

        public UnitMapElement(ICommandQueue commandQueue, IUnit unit, IntVector2 tileCoords, IMutableMapSectionData mapSectionData) {
            _unit = unit;
            _tileCoords = tileCoords;
            _mapSectionData = mapSectionData;
            _commandQueue = commandQueue;
        }

        public void HandleDrag(IntVector2 tileCoords) {
            _mapSectionData.RemoveInitialUnit(_tileCoords, _unit.UnitData);
            var tileDistance = tileCoords - _tileCoords;
            _tileCoords = tileCoords;
            _commandQueue.Enqueue<MoveUnitCommand, MoveUnitData>(new MoveUnitData(_unit.UnitId, tileDistance),
                                                                 CommandSource.Game);
            _mapSectionData.AddInitialUnit(_tileCoords, _unit.UnitData);
        }

        public void Remove() {
            _mapSectionData.RemoveInitialUnit(_tileCoords, _unit.UnitData);
            _commandQueue.Enqueue<DespawnUnitCommand, DespawnUnitData>(new DespawnUnitData(_unit.UnitId),
                                                                       CommandSource.Game);
        }
    }
}