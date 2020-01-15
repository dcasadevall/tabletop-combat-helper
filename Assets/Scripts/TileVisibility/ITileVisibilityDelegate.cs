using Math;

namespace TileVisibility {
    /// <summary>
    /// Implementors of this interface will be used to act on changes to tile visibility.
    /// </summary>
    public interface ITileVisibilityDelegate {
        void HandleTileVisibilityChanged(IntVector2 tileCoords, TileVisibilityType tileVisibilityType);
    }
}