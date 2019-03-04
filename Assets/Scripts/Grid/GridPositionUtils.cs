
using UnityEngine;

namespace Grid {
    public class GridPositionUtils {
        /// <summary>
        /// Gets the World position of the tile at the given x and y position in the grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Vector2 GetTileCenterWorldPosition(IGrid grid, int x, int y) {
            float xPosition = grid.WorldSpaceBounds.x + grid.TileSize / 2.0f + grid.TileSize * x;
            float yPosition = grid.WorldSpaceBounds.y + grid.TileSize / 2.0f + grid.TileSize * y;
            
            return new Vector2(xPosition, yPosition);
        }
    }
}