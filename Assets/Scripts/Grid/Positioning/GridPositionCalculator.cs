
using Math;
using UnityEngine;

namespace Grid.Positioning {
    public class GridPositionCalculator : IGridPositionCalculator {
        /// <inheritdoc />
        public Vector2 GetTileCenterWorldPosition(IGrid grid, IntVector2 tileCoords) {
            float xPosition = grid.WorldSpaceBounds().x + grid.TileSize / 2.0f + grid.TileSize * tileCoords.x;
            float yPosition = grid.WorldSpaceBounds().y + grid.TileSize / 2.0f + grid.TileSize * tileCoords.y;
            
            return new Vector2(xPosition, yPosition);
        }

        /// <inheritdoc />
        public Vector2 GetTileOriginWorldPosition(IGrid grid, IntVector2 tileCoords) {
            float x = grid.WorldSpaceBounds().x + grid.TileSize + tileCoords.x;
            float y = grid.WorldSpaceBounds().y + grid.TileSize + tileCoords.y;
            return new Vector2(x, y);
        }

        /// <inheritdoc />
        public IntVector2 GetTileClosestToCenter(IGrid grid) {
            uint xTile = (grid.NumTilesX - 1) / 2;
            uint yTile = (grid.NumTilesY - 1) / 2;
            return IntVector2.Of((int)xTile, (int)yTile);
        }

        /// <inheritdoc />
        public IntVector2? GetTileContainingWorldPosition(IGrid grid, Vector2 worldPosition) {
            int x = GetTileContainingWorldPosition(grid, worldPosition.x, Axis.X, 0, grid.NumTilesX);
            int y = GetTileContainingWorldPosition(grid, worldPosition.y, Axis.Y, 0, grid.NumTilesY);
            if (x == -1 || y == -1) {
                return null;
            }
            
            return IntVector2.Of(x, y);
        }

        /**
         * Recursive method that binary searches the grid in order to find the given world position
         * in the given axis.
         *
         * Returns -1 if the given worldPosition is not contained in the given axis of the grid.
         */
        private int GetTileContainingWorldPosition(IGrid grid, float worldPosition, Axis axis, uint start,
                                                   uint end) {
            float startWorldPosition = GetTileOriginWorldPosition(grid, start, axis);
            float endWorldPosition = GetTileOriginWorldPosition(grid, end, axis);
            if (worldPosition < startWorldPosition || worldPosition > endWorldPosition) {
                return -1;
            }

            if (end <= start) {
                return -1;
            }

            if (end == start + 1) {
                return (int)start;
            }

            uint m = (start + end) / 2;
            float middleWorldPosition = GetTileOriginWorldPosition(grid, m, axis);
            if (worldPosition >= middleWorldPosition) {
                return GetTileContainingWorldPosition(grid, worldPosition, axis, m, end);
            } else {
                return GetTileContainingWorldPosition(grid, worldPosition, axis, start, m);
            }
        }

        /**
         * Helper method to obtain the world position at the tile origin (minx, miny), given an axis.
         */
        private float GetTileOriginWorldPosition(IGrid grid, uint coordinate, Axis axis) {
            if (axis == Axis.X) {
                return GetTileOriginWorldPosition(grid, IntVector2.One * coordinate).x;
            } else {
                return GetTileOriginWorldPosition(grid, IntVector2.One * coordinate).y;
            }
        }
    }
}