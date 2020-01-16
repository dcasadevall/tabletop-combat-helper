using System;

namespace Units.Serialized {
    [Serializable]
    public class UnitStats {
        /// <summary>
        /// Speed used to calculate movement, in feet / turn.
        /// A unit uses 5 feet in order to move a tile, horizontally or vertically.
        /// Terrain may affect this cost.
        /// </summary>
        public int speed = 30;

        /// <summary>
        /// The radius of visibility this unit has.
        /// e.g: How many units (normally feet) in each direction this unit can see.
        /// 5 feet = 1 tile.
        /// </summary>
        public int visibilityRadius = 5;
    }
}