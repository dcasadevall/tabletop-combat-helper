
using Math;
using UnityEngine;

namespace Grid.Positioning {
    public interface IGridPositionCalculator {
        /// <summary>
        /// Gets the World position of the tile at the given x and y position in the grid of the current context.
        /// </summary>
        /// <param name="tileCoords"></param>
        /// <returns></returns>
        Vector2 GetTileCenterWorldPosition(IntVector2 tileCoords);

        /// <summary>
        /// Gets the world position of the tile origin (lower left corner).
        /// </summary>
        /// <returns></returns>
        Vector2 GetTileOriginWorldPosition(IntVector2 tileCoords);
        
        /**
         * Returns the tile that is closest to the center of the grid in the current context.
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
        IntVector2 GetTileClosestToCenter();

        /// <summary>
        /// Gets the tile (if any) containing the given world position.
        /// Returns null if the given worldPosition is not contained in the grid in the current context.
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        IntVector2? GetTileContainingWorldPosition(Vector2 worldPosition);

        /// <summary>
        /// Gets the tiles at a given distance from the given tile.
        /// Diagonal movement is considered distance 2 (1 for horizontal, 1 for vertical)
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        IntVector2[] GetTilesAtDistance(IntVector2 coords, int distance);

        /// <summary>
        /// Returns the tiles contained within the given rectangle, in world space coordinates.
        /// It returns any tiles partially covered, or fully contained within the given rect.
        /// </summary>
        /// <param name="worldSpaceRect"></param>
        /// <returns></returns>
        IntVector2[] GetTilesCoveredByRect(Rect worldSpaceRect);
    }
}