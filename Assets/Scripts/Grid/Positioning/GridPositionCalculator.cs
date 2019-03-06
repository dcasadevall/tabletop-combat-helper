
using UnityEngine;

namespace Grid.Positioning {
    public class GridPositionCalculator : IGridPositionCalculator {
        /// <summary>
        /// Gets the World position of the tile at the given x and y position in the grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector2 GetTileCenterWorldPosition(IGrid grid, int x, int y) {
            float xPosition = grid.WorldSpaceBounds.x + grid.TileSize / 2.0f + grid.TileSize * x;
            float yPosition = grid.WorldSpaceBounds.y + grid.TileSize / 2.0f + grid.TileSize * y;
            
            return new Vector2(xPosition, yPosition);
        }

        /**
         * Returns the tile that is closest to the center of the grid.
         * In the case where the grid is not of odd width (x axis), the tile X returned is the one left of the center position.
         * In the case where the grid is not of odd height (y axis), the tile Y returned is the one bottom of the center position.
         * i.e (o is returned)
         *
         * x x x
         * x o x
         * x x x
         *
         * e.g 2:
         *
         * x x x x
         * x o x x
         * 
         */
        public Vector2 GetTileClosestToCenter(IGrid grid) {
            uint xTile = grid.NumTilesX / 2;
            uint yTile = grid.NumTilesX / 2;
            return new Vector2(xTile, yTile);
        }
    }
}