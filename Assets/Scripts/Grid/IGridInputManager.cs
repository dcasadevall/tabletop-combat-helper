
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
    /// Gets the tile under the mouse's current position, or null if the mouse is outside of the grid.
    /// </summary>
    IntVector2? TileAtMousePosition { get; }
    /// <summary>
    /// Gets the units, if any, at the tile under the mouse's current position.
    /// </summary>
    IUnit[] UnitsAtMousePosition { get; }
  }
}