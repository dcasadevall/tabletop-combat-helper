
using Math;
using Units;

namespace Grid {
    /// <summary>
    /// Implementors of this interface will handle placing units in the provided <see cref="IGrid"/>,
    /// as well as moving those units or querying for their position.
    /// </summary>
    public interface IGridUnitManager {
        /// <summary>
        /// Event called whenever a unit is placed on a tile.
        /// This may be due to initial spawn, or when a unit is moved from one tile to another.
        /// </summary>
        event System.Action<IUnit, IntVector2> UnitPlacedAtTile;
        
        /// <summary>
        /// Gets all the units in the tile at the given tile coordinates.
        /// Order is undetermined.
        /// </summary>
        /// <param name="tileCoords"></param>
        /// <returns></returns>
        IUnit[] GetUnitsAtTile(IntVector2 tileCoords);

        ///  <summary>
        ///  Places the given unit at the tile for the given tile coordinates, if possible.
        ///  Returns true if the unit was successfully place, false otherwise.
        /// 
        ///  If the unit already exists in the grid, this unit is removed from the original tile and placed
        ///  into the  new one, effectively "moving" it.
        ///  </summary>
        ///  <param name="unit"></param>
        /// <param name="tileCoords"></param>
        /// <returns></returns>
        bool PlaceUnitAtTile(IUnit unit, IntVector2 tileCoords);

        /// <summary>
        /// Removes the given unit from the grid.
        /// 
        /// Returns true if the unit was successfully removed, and false otherwise.
        /// </summary>
        /// <param name="unit"></param>
        bool RemoveUnit(IUnit unit);
        
        /// <summary>
        /// Returns the tile coordinates of the given unit in the grid, or null if such unit is not in the grid.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        IntVector2? GetUnitCoords(IUnit unit);
    }
}