
using System.Collections.Generic;

namespace Units {
    public interface IUnitRegistry {
        /// <summary>
        /// Returns all the currently spawned units.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IUnit> GetAllUnits();
        
        /// <summary>
        /// Gets the unit associated to the given unit ID, or null if not found.
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        IUnit GetUnit(UnitId unitId);
    }
}