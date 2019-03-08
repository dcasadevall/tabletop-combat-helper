
using Grid.Serialized;
using UnityEngine;

namespace Grid {
    /// <summary>
    /// A 2D Grid used to position <see cref="Units.IUnit"/> elements.
    /// 
    /// Consumers can use this interface to check a <see cref="Units.IUnit"/>s position,
    /// check line of sight between <see cref="Units.IUnit"/>s and move <see cref="Units.IUnit"/>s from
    /// one position to another.
    ///
    /// This grid is assumed to be rectangular and of square tile size.
    /// X and Y axes are assumed to be oriented left to right and bottom to top.
    /// No rotation is supported for now.
    /// </summary>
    public interface IGrid {
        /// <summary>
        /// The number of tiles in the X axis.
        /// NumTilesX will always be >= 1.
        /// </summary>
        uint NumTilesX { get; }
        /// <summary>
        /// The number of tiles in the Y axis.
        /// NumTilesY will always be >= 1.
        /// </summary>
        uint NumTilesY { get; }
        /// <summary>
        /// How many units (in Unity, 1 unit is 1 meter) a tile measures.
        /// We intentionally made this an integer to avoid floating point errors and for simplicity.
        /// 
        /// TileSize will always be >=1.
        /// </summary>
        uint TileSize { get; }
        /// <summary>
        /// Loads the given <see cref="GridData"/>, setting values like grid dimensions.
        /// </summary>
        /// <param name="data"></param>
        void LoadGridData(IGridData data);
    }

    public static class GridExtensions {
        /// <summary>
        /// The bounds of the rectangle containing this grid, in Unity World Space coordinates.
        /// </summary>
        public static Rect WorldSpaceBounds(this IGrid grid) {
            return new Rect(-grid.TileSize * grid.NumTilesX / 2.0f,
                            -grid.TileSize * grid.NumTilesY / 2.0f,
                            grid.NumTilesX * grid.TileSize,
                            grid.NumTilesY * grid.TileSize); 
        }
		/// <summary>
        /// The bounds of the rectangle containing this grid, in tile coordinates.
        /// </summary>
        public static Rect TileBounds(this IGrid grid) {
            return new Rect(Vector2.zero,
                            new Vector2(grid.NumTilesX, grid.NumTilesY));
    	}
	}
}
