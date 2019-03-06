
using Units;

namespace Grid {
    /// <summary>
    /// Implementors of this interface will handle placing units in the provided <see cref="IGrid"/>,
    /// as well as moving those units or querying for their position.
    /// </summary>
    public interface IGridUnitManager {
        /// <summary>
        /// Gets all the units in the tile at the given x and y coordinate.
        /// Order is undetermined.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        UnitId[] GetUnitsAtTile(int x, int y);
        
        /// <summary>
        /// Places the given unit at the tile for the given x and y coordinates, if possible.
        /// Returns true if the unit was successfully place, false otherwise.
        ///
        /// If the unit already exists in the grid, this unit is removed from the original tile and placed
        /// into the  new one, effectively "moving" it.
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        bool PlaceUnitAtTile(UnitId unit, int x, int y);
    }
}