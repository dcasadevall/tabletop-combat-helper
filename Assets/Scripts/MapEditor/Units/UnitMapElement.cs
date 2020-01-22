using Grid;
using MapEditor.MapElement;
using Units;

namespace MapEditor.Units {
    public class UnitMapElement : IMapElement {
        private readonly IGridUnitManager _gridUnitManager;
        private readonly IUnit _unit;

        public UnitMapElement(IGridUnitManager gridUnitManager, IUnit unit) {
            _gridUnitManager = gridUnitManager;
            _unit = unit;
        }

        public void Remove() {
            _gridUnitManager.RemoveUnit(_unit);
        }
    }
}