
using System.Collections.Generic;
using Grid;
using Units.Serialized;
using UnityEngine;

namespace Units {
    /// <summary>
    /// A Unit represents a player or npc in the tabletop game.
    /// It can be position in the <see cref="IGrid"/> and interacted with.
    /// </summary>
    public interface IUnit {
        /// <summary>
        /// A unique identifier representing this unit.
        /// </summary>
        UnitId UnitId { get; }
        
        /// <summary>
        /// The data this unit was constructed with.
        /// </summary>
        IUnitData UnitData { get; }
        
        /// <summary>
        /// An array containing the units that are pets (or companions) to this unit.
        /// Pet Units are directly associated to this unit, and should be initially spawned together.
        /// </summary>
        IUnit[] PetUnits { get; }
    }

    public static class UnitExtensions {
        /// <summary>
        /// Returns all descendants from this unit, as well as this unit.
        /// This includes all pet units, and all pet units of those pet units, etc..
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static List<IUnit> GetUnitsInHierarchy(this IUnit unit) {
            List<IUnit> unitsInHierarchy = new List<IUnit>();
            unitsInHierarchy.Add(unit);
            foreach (var petUnit in unit.PetUnits) {
                unitsInHierarchy.AddRange(petUnit.GetUnitsInHierarchy());
            }

            return unitsInHierarchy;
        }
    }
}