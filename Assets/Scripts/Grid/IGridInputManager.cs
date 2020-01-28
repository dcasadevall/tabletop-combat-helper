using System;
using Math;
using Units;

namespace Grid {
    public interface IGridInputManager {
        /// <summary>
        /// Observable which receives data when the mouse has entered a new tile.
        /// This is useful for dealing with streams which need to observe the currently hovered tile.
        /// </summary>
        IObservable<IntVector2> MouseEnteredTile { get; }

        /// <summary>
        /// Observable which receives data when the left mouse button has been pressed on a tile (down and up).
        /// Mouse Down starting on tile A but ending on tile B is not considered a mouse button press.
        /// This observable also emits events for long mouse holds, as long as hey start and end on the same tile.
        /// </summary>
        IObservable<IntVector2> LeftMouseButtonOnTile { get; }

        /// <summary>
        /// Observable which receives data the frame in which the left mouse button has been pressed down on a tile.
        /// </summary>
        IObservable<IntVector2> LeftMouseDownOnTile { get; }

        /// <summary>
        /// Observable which receives data the frames in which the left mouse button is held down while the mouse
        /// is being moved.
        /// </summary>
        IObservable<IntVector2> LeftMouseDragOnTile { get; }
        
        /// <summary>
        /// Gets the tile under the mouse's current position, or null if the mouse is outside of the grid.
        /// </summary>
        IntVector2? TileAtMousePosition { get; }
    }
}