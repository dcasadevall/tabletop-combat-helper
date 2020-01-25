using CommandSystem;
using Grid;
using MapEditor.MapElement;
using Units;
using Units.Spawning.Commands;

namespace MapEditor.Units {
    public class UnitMapElement : IMapElement {
        private readonly IUnit _unit;
        private readonly ICommandQueue _commandQueue;

        public UnitMapElement(ICommandQueue commandQueue, IUnit unit) {
            _unit = unit;
            _commandQueue = commandQueue;
        }

        public void Remove() {
            _commandQueue.Enqueue<DespawnUnitCommand, DespawnUnitData>(new DespawnUnitData(_unit.UnitId),
                                                                       CommandSource.Game);
        }
    }
}