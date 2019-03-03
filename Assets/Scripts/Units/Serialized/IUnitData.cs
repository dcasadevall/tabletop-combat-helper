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