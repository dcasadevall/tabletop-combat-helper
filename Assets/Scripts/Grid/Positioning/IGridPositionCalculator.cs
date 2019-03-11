
using Math;
using UnityEngine;

namespace Grid.Positioning {
    public interface IGridPositionCalculator {
        /// <summary>
        /// Gets the World position of the tile at the given x and y position in the grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="tileCoords"></param>
        /// <returns></returns>
        Vector2 GetTileCenterWorldPosition(IGrid grid, IntVector2 tileCoords);
        
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
        IntVector2 GetTileClosestToCenter(IGrid grid);

        /// <summary>
        /// Gets the tile (if any) containing the given world position.
        /// Returns null if the given worldPosition is not contained in the grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        IntVector2? GetTileContainingWorldPosition(IGrid grid, Vector2 worldPosition);
    }
}