using System;
using UnityEngine;

namespace Grid.Serialized {
    /// <summary>
    /// ScriptableObject used to configure data for the grid model.
    /// This allows art to be bound to the data model.
    /// </summary>
    [Serializable]
    public class GridData : IGridData {
        public uint numTilesX = 20;
        public uint NumTilesX {
            get {
                return numTilesX;
            }
        }

        public uint numTilesY = 20;
        public uint NumTilesY {
            get {
                return numTilesY;
            }
        }

        public bool centeredAtWorldZero = true;
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