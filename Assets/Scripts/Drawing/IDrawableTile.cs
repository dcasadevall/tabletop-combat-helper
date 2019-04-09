using UnityEngine;

namespace Drawing {
    /// <summary>
    /// An individual drawable tile that will hold the read / write sprite to be used with paint commands.
    /// It is instantiated by the <see cref="IDrawableTileRegistry"/> if needed.
    /// </summary>
    public interface IDrawableTile {
        /// <summary>
        /// The read / write sprite used for this tile.
        /// </summary>
        Sprite Sprite { get; }
    }
}