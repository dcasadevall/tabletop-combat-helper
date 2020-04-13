using System;
using UnityEngine;

namespace Utils.DataStructures {
    /// <summary>
    /// A Sprite with a Weight value for randomization.
    /// </summary>
    [Serializable]
    public struct WeightedSprite
    {
        /// <summary>
        /// Sprite.
        /// </summary>
        public Sprite Sprite;
        /// <summary>
        /// Weight of the Sprite.
        /// </summary>
        public int Weight;
    }
}
