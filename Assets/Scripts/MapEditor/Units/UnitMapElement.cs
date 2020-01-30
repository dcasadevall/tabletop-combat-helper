using CommandSystem;
using Grid;
using Grid.Commands;
using MapEditor.MapElement;
using Math;
using Units;
using Units.Spawning.Commands;

namespace MapEditor.Units {
    public class UnitMapElement : IMapElement {
        private readonly IUnit _unit;
        private readonly ICommandQueue _commandQueue;

        private IntVector2 _tileCoords;

        public UnitMapElement(ICommandQueue commandQueue, IUnit unit, IntVector2 tileCoords) {
            _unit = unit;
            _tileCoords = tileCoords;
            _commandQueue = commandQueue;
        }

        public void HandleDrag(IntVector2 tileCoords) {
            var tileDistance = tileCoords - _tileCoords;
            _tileCoords = tileCoords;
            _commandQueue.Enqueue<MoveUnitCommand, MoveUnitData>(new MoveUnitData(_unit.UnitId, tileDistance),
                                                                 CommandSource.Game);
        }

        public void Remove() {
            _commandQueue.Enqueue<DespawnUnitCommand, DespawnUnitData>(new DespawnUnitData(_unit.UnitId),
                                                                       CommandSource.Game);
        }
    }
}