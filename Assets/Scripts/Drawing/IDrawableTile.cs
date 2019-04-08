using UnityEngine;

namespace Drawing {
    /// <summary>
    /// An individual drawable tile that will receive mouse input events from <see cref="DrawingInputManager"/>.
    /// It is instantiated by the <see cref="IDrawableTileRegistry"/> if needed.
    /// </summary>
    public interface IDrawableTile {
        /// <summary>
        /// Event fired on the frame the left mouse button is pressed.
        /// The given point is in the tile's relative position.
        /// </summary>
        /// <param name="point"></param>
        void HandleMouseDown(Vector2 point);
        /// <summary>
        /// Event fired every frame the left mouse button is held down.
        /// The given point is in the tile's relative position.
        /// </summary>
        /// <param name="point"></param>
        void HandleMouseDrag(Vector2 point);
        /// <summary>
        /// Event fired on the frame the left mouse button is released.
        /// The given point is in the tile's relative position.
        /// </summary>
        /// <param name="point"></param>
        void HandleMouseUp(Vector2 point);

        /// <summary>
        /// Clears all drawings in this tile.
        /// </summary>
        void Clear();
    }
}