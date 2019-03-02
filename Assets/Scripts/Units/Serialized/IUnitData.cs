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
    }
}