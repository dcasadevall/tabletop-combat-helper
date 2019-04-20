using System.Collections.Generic;
using Math;
using Vector2 = UnityEngine.Vector2;

namespace Drawing.DrawableTiles {
    /// <summary>
    /// Keeps track of <see cref="IDrawableTile"/>s in each tile on the <see cref="Grid.IGrid"/>.
    /// Creates new <see cref="IDrawableTile"/> when necessary.
    /// </summary>
    public interface IDrawableTileRegistry {
        /// <summary>
        /// Gets the <see cref="IDrawableTile"/> at the given <see cref="tileCoords"/>.
        /// Returns null if the given coords are not inside the current grids bounds.
        ///
        /// Creates a new <see cref="IDrawableTile"/> if one doesn't exist yet at those coordinates.
        /// </summary>
        /// <returns></returns>
        IDrawableTile GetDrawableTileAtCoordinates(IntVector2 tileCoords);

        /// <summary>
        /// Returns all drawable tiles that have been drawn on so far.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDrawableTile> GetAllTiles();

        /// <summary>
        /// Given a world position, return the local position relative to the coordinates of the tile
        /// containing this world position.
        ///
        /// Returns null if no tile contains the given world position.
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        Vector2? GetLocalPosition(Vector2 worldPosition);
    }
}