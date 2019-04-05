using Math;

namespace Drawing {
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
    }
}