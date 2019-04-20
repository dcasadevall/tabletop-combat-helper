using UnityEngine;

namespace Units.Serialized {
    /// <summary>
    /// Data associated with a given unit.
    /// </summary>
    public interface IUnitData {
        /// <summary>
        /// Name of this unit.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Type of the unit (Player vs NonPlayer).
        /// </summary>
        UnitType UnitType { get; }
        
        /// <summary>
        /// An array containing the unit data for pets that are permanent to this unit.
        /// This means the pets found in this array will always spawn with this unit.
        /// </summary>
        IUnitData[] Pets { get; }
        
        /// <summary>
        /// Sprite associated with this unit.
        /// </summary>
        Sprite Sprite { get; }
        
        /// <summary>
        /// Sprite associated with this unit's avatar icon.
        /// Should be shown in minimap and when zoomed out.
        /// </summary>
        Sprite AvatarSprite { get; }
    }
}