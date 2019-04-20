
using Math;

namespace Grid {
  public interface IGridInputManager {
    /// <summary>
    /// Gets the tile under the mouse's current position, or null if the mouse is outside of the grid.
    /// </summary>
    /// <returns></returns>
    IntVector2? GetTileAtMousePosition();
  }
}