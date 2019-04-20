using System;
using Grid;
using Grid.Positioning;
using Logging;
using Math;
using Math.Random;
using UnityEngine;
using ILogger = Logging.ILogger;

namespace Map.Rendering {
    /// <summary>
    /// Randomly place tiles in the context's <see cref="IMapData"/> into the grid.
    /// Tiles may be rotated randomly
    /// </summary>
    public class RandomizedRepeatedTileLoader : ITileLoader {
        private readonly IRandomProvider _randomProvider;
        private readonly ILogger _logger;
        private readonly IMapData _mapData;
        private readonly IGridPositionCalculator _positionCalculator;
        private readonly TileRendererBehaviour.Pool _tileRendererPool;

        public RandomizedRepeatedTileLoader(IRandomProvider randomProvider, 
                                            ILogger logger,
                                            IMapData mapData,
                                            IGridPositionCalculator positionCalculator,
                                            TileRendererBehaviour.Pool tileRendererPool) {
            _randomProvider = randomProvider;
            _logger = logger;
            _mapData = mapData;
            _positionCalculator = positionCalculator;
            _tileRendererPool = tileRendererPool;
        }
        
        public void LoadTiles() {
            if (_mapData.Sprites == null || _mapData.Sprites.Length == 0) {
                _logger.LogError(LoggedFeature.Map, "Map Data has no sprites");
                return;
            }
            
            uint x = 0;
            uint y = 0;
            while (y < _mapData.GridData.NumTilesY) {
                x = 0;
                uint miny = UInt32.MaxValue;
                while (x < _mapData.GridData.NumTilesX) {
                    Sprite sprite =
                        _mapData.Sprites[_randomProvider.GetRandomIntegerInRange(0, _mapData.Sprites.Length)];
                    TileRendererBehaviour tileRendererBehaviour = _tileRendererPool.Spawn(sprite);
                    tileRendererBehaviour.SpriteRenderer.flipX = _randomProvider.GetRandomIntegerInRange(0, 2) == 0;
                    tileRendererBehaviour.SpriteRenderer.flipY = _randomProvider.GetRandomIntegerInRange(0, 2) == 0;
                    tileRendererBehaviour.transform.position =
                        _positionCalculator.GetTileOriginWorldPosition(IntVector2.Of(x, y)) +
                        new Vector2(sprite.bounds.extents.x, sprite.bounds.extents.y);
                    
                    
                    // Calculate how many units in X this unit generates (assume square for now).
                    uint numXTiles = (uint)Mathf.CeilToInt(sprite.bounds.size.x / _mapData.PixelsPerUnit);
                    uint numYTiles = (uint)Mathf.CeilToInt(sprite.bounds.size.y / _mapData.PixelsPerUnit);
                    x += numXTiles;
                    miny = System.Math.Min(numYTiles, miny);
                }

                y += miny;
            }
        }
    }
}