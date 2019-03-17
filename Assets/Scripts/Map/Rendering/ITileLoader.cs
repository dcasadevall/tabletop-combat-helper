namespace Map.Rendering {
    /// <summary>
    /// Loads the tiles in the current map context.
    ///
    /// Implementations will be chosen based on the <see cref="MapTileType"/> in context.
    /// </summary>
    public interface ITileLoader {
        void LoadTiles();
    }
}