using System;
using Grid.Positioning;
using Logging;
using Math;
using UnityEngine;
using ILogger = Logging.ILogger;

namespace Map.Rendering {
    public class SequentialUniqueTileLoader : ITileLoader {
        private readonly ILogger _logger;
        private readonly IMapSectionData _mapSectionData;
        private readonly IGridPositionCalculator _positionCalculator;
        private readonly TileRendererBehaviour.Pool _tileRendererPool;

        public SequentialUniqueTileLoader(ILogger logger,
                              IMapSectionData mapSectionData,
                              IGridPositionCalculator positionCalculator,
                              TileRendererBehaviour.Pool tileRendererPool) {
            _logger = logger;
            _mapSectionData = mapSectionData;
            _positionCalculator = positionCalculator;
            _tileRendererPool = tileRendererPool;
        }

        public void LoadTiles() {
            if (_mapSectionData.Sprites == null || _mapSectionData.Sprites.Length == 0) {
                _logger.LogError(LoggedFeature.Map, "Map Data has no sprites");
                return;
            }

            uint x = 0;
            uint y = 0;
            uint miny = UInt32.MaxValue;
            int i = 0;
            while (i < _mapSectionData.Sprites.Length && y < _mapSectionData.GridData.NumTilesY) {
                while (i < _mapSectionData.Sprites.Length && x < _mapSectionData.GridData.NumTilesX) {
                    Sprite sprite = _mapSectionData.Sprites[i];
                    TileRendererBehaviour tileRendererBehaviour = _tileRendererPool.Spawn(sprite);
                    tileRendererBehaviour.transform.position =
                        _positionCalculator.GetTileOriginWorldPosition(IntVector2.Of(x, y)) +
                        new Vector2(sprite.bounds.extents.x, sprite.bounds.extents.y);

                    // Calculate how many units in X this unit generates (assume square for now).
                    uint numXTiles = (uint) Mathf.CeilToInt(sprite.bounds.size.x / _mapSectionData.PixelsPerUnit);
                    uint numYTiles = (uint) Mathf.CeilToInt(sprite.bounds.size.y / _mapSectionData.PixelsPerUnit);
                    x += numXTiles;
                    miny = System.Math.Min(numYTiles, miny);
                }

                y += miny;
            }
        }
    }
}