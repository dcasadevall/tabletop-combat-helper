using System;
using Grid;
using Grid.Positioning;
using Logging;
using Math;
using Math.Random;
using UnityEngine;
using ILogger = Logging.ILogger;

namespace Map.Rendering {
    public class RandomizedRepeatedTileLoader : ITileLoader {
        private readonly IRandomProvider _randomProvider;
        private readonly ILogger _logger;
        private readonly IGrid _grid;
        private readonly IGridPositionCalculator _positionCalculator;
        private readonly TileRendererBehaviour.Pool _tileRendererPool;

        public RandomizedRepeatedTileLoader(IRandomProvider randomProvider, ILogger logger,
                                            IGrid grid,
                                            IGridPositionCalculator positionCalculator,
                                            TileRendererBehaviour.Pool tileRendererPool) {
            _randomProvider = randomProvider;
            _logger = logger;
            _grid = grid;
            _positionCalculator = positionCalculator;
            _tileRendererPool = tileRendererPool;
        }
        
        public void LoadTiles(IMapData mapData) {
            if (mapData.Sprites == null || mapData.Sprites.Length == 0) {
                _logger.LogError(LoggedFeature.Map, "Map Data has no sprites");
                return;
            }
            
            uint x = 0;
            uint y = 0;
            while (y < mapData.GridData.NumTilesY) {
                x = 0;
                uint miny = UInt32.MaxValue;
                while (x < mapData.GridData.NumTilesX) {
                    Sprite sprite =
                        mapData.Sprites[_randomProvider.GetRandomIntegerInRange(0, mapData.Sprites.Length)];
                    TileRendererBehaviour tileRendererBehaviour = _tileRendererPool.Spawn(sprite);
                    tileRendererBehaviour.SpriteRenderer.flipX = _randomProvider.GetRandomIntegerInRange(0, 2) == 0;
                    tileRendererBehaviour.SpriteRenderer.flipY = _randomProvider.GetRandomIntegerInRange(0, 2) == 0;
                    tileRendererBehaviour.transform.position =
                        GetTileOriginWorldPosition(IntVector2.Of(x, y)) +
                        new Vector3(sprite.bounds.extents.x, sprite.bounds.extents.y, 0) - Vector3.one;
                    
                    
                    // Calculate how many units in X this unit generates (assume square for now).
                    uint numXTiles = (uint)Mathf.CeilToInt(sprite.bounds.size.x / mapData.PixelsPerUnit);
                    uint numYTiles = (uint)Mathf.CeilToInt(sprite.bounds.size.y / mapData.PixelsPerUnit);
                    x += numXTiles;
                    miny = System.Math.Min(numYTiles, miny);
                }

                y += miny;
            }
        }

        /**
        * Helper method to obtain the world position at the tile origin (minx, miny)
        * TODO: Move this to a helper method.
        */
        private Vector3 GetTileOriginWorldPosition(IntVector2 tilecoords) {
            float x = _grid.WorldSpaceBounds().x + _grid.TileSize + tilecoords.x;
            float y = _grid.WorldSpaceBounds().y + _grid.TileSize + tilecoords.y;
            return new Vector2(x, y);
        }
    }
}