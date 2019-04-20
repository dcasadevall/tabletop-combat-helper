using System;
using Grid.Positioning;
using Logging;
using Math;
using UnityEngine;
using ILogger = Logging.ILogger;

namespace Map.Rendering {
    public class SequentialUniqueTileLoader : ITileLoader {
        private readonly ILogger _logger;
        private readonly IMapData _mapData;
        private readonly IGridPositionCalculator _positionCalculator;
        private readonly TileRendererBehaviour.Pool _tileRendererPool;

        public SequentialUniqueTileLoader(ILogger logger,
                              IMapData mapData,
                              IGridPositionCalculator positionCalculator,
                              TileRendererBehaviour.Pool tileRendererPool) {
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
            uint miny = UInt32.MaxValue;
            int i = 0;
            while (i < _mapData.Sprites.Length && y < _mapData.GridData.NumTilesY) {
                while (i < _mapData.Sprites.Length && x < _mapData.GridData.NumTilesX) {
                    Sprite sprite = _mapData.Sprites[i];
                    TileRendererBehaviour tileRendererBehaviour = _tileRendererPool.Spawn(sprite);
                    tileRendererBehaviour.transform.position =
                        _positionCalculator.GetTileOriginWorldPosition(IntVector2.Of(x, y)) +
                        new Vector2(sprite.bounds.extents.x, sprite.bounds.extents.y);

                    // Calculate how many units in X this unit generates (assume square for now).
                    uint numXTiles = (uint) Mathf.CeilToInt(sprite.bounds.size.x / _mapData.PixelsPerUnit);
                    uint numYTiles = (uint) Mathf.CeilToInt(sprite.bounds.size.y / _mapData.PixelsPerUnit);
                    x += numXTiles;
                    miny = System.Math.Min(numYTiles, miny);
                }

                y += miny;
            }
        }
    }
}