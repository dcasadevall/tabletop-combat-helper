
namespace Map.Rendering {
  /// <summary>
  /// Defines the different generation algorithms when creating map tiles.
  /// </summary>
  public enum MapTileType {
    /// <summary>
    /// Assumes one or more sprites are given.
    /// Repeats these tiles in random order / rotation
    /// filling the grid's size.
    /// </summary>
    RandomizeRepeatedTiles,
    /// <summary>
    /// Given an ordered list of tiles, places them sequentially,
    /// stopping when all of them have been place once.
    /// </summary>
    SequentialUniqueTiles,
  }
}