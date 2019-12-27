using System;

namespace Units.Serialized {
    [Serializable]
    public class UnitStats {
        /// <summary>
        /// Speed used to calculate movement.
        /// A unit uses 5 speed units in order to move a tile, horizontally or vertically.
        /// Terrain may affect this cost.
        /// </summary>
        public int speed = 30;
    }
}