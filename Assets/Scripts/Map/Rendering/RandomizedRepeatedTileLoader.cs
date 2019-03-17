using System;
using Logging;
using Math.Random;
using UnityEngine;
using ILogger = Logging.ILogger;

namespace Map.Rendering {
    public class RandomizedRepeatedTileLoader : ITileLoader {
        private readonly IRandomProvider _randomProvider;
        private readonly ILogger _logger;
        private readonly TileRendererBehaviour.Pool _tileRendererPool;

        public RandomizedRepeatedTileLoader(IRandomProvider randomProvider, ILogger logger,
                                            TileRendererBehaviour.Pool tileRendererPool) {
            _randomProvider = randomProvider;
            _logger = logger;
            _tileRendererPool = tileRendererPool;
        }
        
        public void LoadTiles(IMapData mapData) {
            if (mapData.Sprites.Length == 0) {
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
                    _tileRendererPool.Spawn(sprite);
                    
                    // Calculate how many units in X this unit generates (assume square for now).
                    uint numXTiles = (uint)Mathf.CeilToInt(sprite.bounds.size.x / mapData.PixelsPerUnit);
                    uint numYTiles = (uint)Mathf.CeilToInt(sprite.bounds.size.y / mapData.PixelsPerUnit);
                    x += numXTiles;
                    miny = System.Math.Min(numYTiles, miny);
                }

                y += miny;
            }
        }
    }
}