using UnityEngine;

namespace Grid.Serialized {
    /// <summary>
    /// ScriptableObject used to configure data for the grid model.
    /// This allows art to be bound to the data model.
    /// </summary>
    public class GridData : ScriptableObject {
        public uint numTilesX;
        public uint numTilesY;
    }
}