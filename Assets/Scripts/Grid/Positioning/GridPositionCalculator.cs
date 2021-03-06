using System;
using System.Collections.Generic;
using Math;
using UnityEngine;
using Utils;

namespace Grid.Positioning {
    public class GridPositionCalculator : IGridPositionCalculator {
        private readonly IGrid _grid;

        public GridPositionCalculator(IGrid grid) {
            _grid = grid;
        }

        /// <inheritdoc />
        public Vector2 GetTileCenterWorldPosition(IntVector2 tileCoords) {
            float xPosition = _grid.WorldSpaceBounds().x + _grid.TileSize / 2.0f + _grid.TileSize * tileCoords.x;
            float yPosition = _grid.WorldSpaceBounds().y + _grid.TileSize / 2.0f + _grid.TileSize * tileCoords.y;

            return new Vector2(xPosition, yPosition);
        }

        /// <inheritdoc />
        public Vector2 GetTileOriginWorldPosition(IntVector2 tileCoords) {
            float x = _grid.WorldSpaceBounds().x + _grid.TileSize * tileCoords.x;
            float y = _grid.WorldSpaceBounds().y + _grid.TileSize * tileCoords.y;
            return new Vector2(x, y);
        }

        /// <inheritdoc />
        public IntVector2 GetTileClosestToCenter() {
            uint xTile = (_grid.NumTilesX - 1) / 2;
            uint yTile = (_grid.NumTilesY - 1) / 2;
            return IntVector2.Of((int) xTile, (int) yTile);
        }

        /// <inheritdoc />
        public IntVector2? GetTileContainingWorldPosition(Vector2 worldPosition) {
            int x = GetTileContainingWorldPosition(worldPosition.x, Axis.X, 0, _grid.NumTilesX);
            int y = GetTileContainingWorldPosition(worldPosition.y, Axis.Y, 0, _grid.NumTilesY);
            if (x == -1 || y == -1) {
                return null;
            }

            return IntVector2.Of(x, y);
        }

        /// <inheritdoc />
        public IntVector2[] GetTilesAtDistance(IntVector2 coords, int distance) {
            if (distance <= 0) {
                return new IntVector2[0];
            }

            var tiles = new List<IntVector2>();
            for (int x = -distance; x <= distance; x++) {
                for (int y = -distance; y <= distance; y++) {
                    if (x == 0 && y == 0) {
                        continue;
                    }

                    var tileCoords = coords + IntVector2.Of(x, y);
                    if (!IsInsideGrid(tileCoords)) {
                        continue;
                    }

                    if (System.Math.Abs(x) + System.Math.Abs(y) > distance) {
                        continue;
                    }

                    tiles.Add(tileCoords);
                }
            }

            return tiles.ToArray();
        }

        /// <inheritdoc />
        public IntVector2[] GetTilesCoveredByRect(Rect worldSpaceRect) {
            float minX = Mathf.Max(GetTileOriginWorldPosition(0, Axis.X), worldSpaceRect.min.x);
            float maxX = Mathf.Min(GetTileOriginWorldPosition(_grid.NumTilesX - 1, Axis.X) + _grid.TileSize,
                                   worldSpaceRect.max.x);
            float minY = Mathf.Max(GetTileOriginWorldPosition(0, Axis.Y), worldSpaceRect.min.y);
            float maxY = Mathf.Min(GetTileOriginWorldPosition(_grid.NumTilesY - 1, Axis.Y) + _grid.TileSize,
                                   worldSpaceRect.max.y);

            IntVector2 bottomLeft = GetTileContainingWorldPosition(new Vector2(minX, minY)).GetValueChecked();
            Vector2 bottomLeftCenter = GetTileCenterWorldPosition(bottomLeft);
            IntVector2 topRight = GetTileContainingWorldPosition(new Vector2(maxX, maxY)).GetValueChecked();
            Vector2 topRightCenter = GetTileCenterWorldPosition(topRight);

            // Only select tiles covered at least by their center position.
            if (minX > bottomLeftCenter.x) {
                minX += _grid.TileSize;
            }

            if (minY > bottomLeftCenter.y) {
                minY += _grid.TileSize;
            }

            if (maxX < topRightCenter.x) {
                maxX -= _grid.TileSize;
            }

            if (maxY < topRightCenter.y) {
                maxY -= _grid.TileSize;
            }

            // Guarantee that we have not gone outside of grid bounds after correcting the selection.
            Rect gridBounds = _grid.WorldSpaceBounds();
            if (minX >= gridBounds.xMax ||
                minY >= gridBounds.yMax ||
                maxX <= gridBounds.xMin ||
                maxY <= gridBounds.yMin) {
                return new IntVector2[0];
            }

            // Assign new bottomLeft / topRight with constrained bounds
            bottomLeft = GetTileContainingWorldPosition(new Vector2(minX, minY)).GetValueChecked();
            topRight = GetTileContainingWorldPosition(new Vector2(maxX, maxY)).GetValueChecked();
            
            var tiles = new List<IntVector2>();
            for (int x = bottomLeft.x; x <= topRight.x; x++) {
                for (int y = bottomLeft.y; y <= topRight.y; y++) {
                    tiles.Add(IntVector2.Of(x, y));
                }
            }

            return tiles.ToArray();
        }

        /**
         * Recursive method that binary searches the _grid in order to find the given world position
         * in the given axis.
         *
         * Returns -1 if the given worldPosition is not contained in the given axis of the _grid.
         */
        private int GetTileContainingWorldPosition(float worldPosition, Axis axis, uint start,
                                                   uint end) {
            float startWorldPosition = GetTileOriginWorldPosition(start, axis);
            float endWorldPosition = GetTileOriginWorldPosition(end, axis);
            if (worldPosition < startWorldPosition || worldPosition > endWorldPosition) {
                return -1;
            }

            if (end <= start) {
                return -1;
            }

            if (end == start + 1) {
                return (int) start;
            }

            uint m = (start + end) / 2;
            float middleWorldPosition = GetTileOriginWorldPosition(m, axis);
            if (worldPosition >= middleWorldPosition) {
                return GetTileContainingWorldPosition(worldPosition, axis, m, end);
            } else {
                return GetTileContainingWorldPosition(worldPosition, axis, start, m);
            }
        }

        /**
         * Helper method to obtain the world position at the tile origin (minx, miny), given an axis.
         */
        private float GetTileOriginWorldPosition(uint coordinate, Axis axis) {
            if (axis == Axis.X) {
                return GetTileOriginWorldPosition(IntVector2.One * coordinate).x;
            } else {
                return GetTileOriginWorldPosition(IntVector2.One * coordinate).y;
            }
        }

        private bool IsInsideGrid(IntVector2 point) {
            return point.x >= 0
                && point.x < _grid.NumTilesX
                && point.y >= 0
                && point.y < _grid.NumTilesY;
        }
    }
}