using JetBrains.Annotations;
using UnityEngine;

namespace Grid.Serialized {
    /// <summary>
    /// ScriptableObject used to configure data for the grid model.
    /// This allows art to be bound to the data model.
    /// </summary>
    public class GridData : ScriptableObject, IGridData {
        public uint numTilesX;
        public uint NumTilesX {
            get {
                return numTilesX;
            }
        }

        public uint numTilesY;
        public uint NumTilesY {
            get {
                return numTilesY;
            }
        }

        public bool centeredAtWorldZero;
        public Vector2 originWorldPosition;
        public Vector2? OriginWorldPosition {
            get {
                if (centeredAtWorldZero) {
                    return null;
                }

                return originWorldPosition;
            }
        }
    }
}