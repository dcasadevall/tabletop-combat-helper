using System;

namespace Drawing.DrawableTiles {
    [Serializable]
    public class DrawbleSpriteSettings {
        /// <summary>
        /// The path under a Resources folder where the drawable tile assets live.
        /// </summary>
        public string path = "Sprites/Drawing";

        /// <summary>
        /// The format to use when creating drawble tiles.
        /// Where 0 is the resources path, and 1 is the sprite index.
        /// </summary>
        public string format = "{0}/DrawableTile_{1}";
    }
}