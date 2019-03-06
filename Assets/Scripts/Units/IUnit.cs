
using Grid;
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
        /// An array containing the units that are pets (or companions) to this unit.
        /// Pet Units are directly associated to this unit, and should be initially spawned together.
        /// </summary>
        IUnit[] PetUnits { get; }
    }
}