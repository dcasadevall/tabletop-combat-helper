using Units;

namespace Grid.GridUnits {
    public interface IGridUnitInputManager {
        /// <summary>
        /// Gets the units, if any, at the tile under the mouse's current position.
        /// </summary>
        IUnit[] UnitsAtMousePosition { get; }
    }
}