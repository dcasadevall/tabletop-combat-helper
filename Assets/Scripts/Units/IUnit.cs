
using System.Collections.Generic;
using Grid;
using Math;
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
        
        /// <summary>
        /// The read-only information of the unit's transform.
        /// Actors accessing this unit should not be able to mutate its transform.
        /// Instead, a <see cref="ITransformableUnit"/> is available to those systems that should be
        /// able to mutate the unit's transform.
        /// </summary>
        TransformData TransformData { get; }
    }
}