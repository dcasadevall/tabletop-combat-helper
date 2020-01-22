using System;
using Grid.Positioning;
using Logging;
using Map.MapData;
using Map.MapSections;
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

            uint y = 0;
            int i = 0;
            uint yIndex = 0;
            uint numTilesX = _mapSectionData.GridData.NumTilesX;
            uint numTilesY = _mapSectionData.GridData.NumTilesY;
            
            Sprite firstSprite = _mapSectionData.Sprites[0];
            
            // Calculate how many units in X this unit generates
            // For now, we assume that all sprites are the same size.
            uint numTilesInSpriteX = (uint) Mathf.CeilToInt(firstSprite.bounds.size.x / _mapSectionData.PixelsPerUnit);
            uint numTilesInSpriteY = (uint) Mathf.CeilToInt(firstSprite.bounds.size.y / _mapSectionData.PixelsPerUnit);
            
            uint numSpritesX = (uint)Mathf.CeilToInt(numTilesX / (float)numTilesInSpriteX);
            uint numSpritesY = (uint)Mathf.CeilToInt(numTilesY / (float)numTilesInSpriteY);
            
            while (i < _mapSectionData.Sprites.Length && y < numTilesY) {
                uint x = 0;
                uint xIndex = 0;
                uint miny = UInt32.MaxValue;
                while (i < _mapSectionData.Sprites.Length && x < numTilesX) {
                    uint index = (numSpritesY - 1 - yIndex) * numSpritesX + xIndex;
                    Sprite sprite = _mapSectionData.Sprites[index];
                    TileRendererBehaviour tileRendererBehaviour = _tileRendererPool.Spawn(sprite);
                    tileRendererBehaviour.transform.position =
                        _positionCalculator.GetTileOriginWorldPosition(IntVector2.Of(x, y)) +
                        new Vector2(sprite.bounds.extents.x, sprite.bounds.extents.y);
                    
                    x += numTilesInSpriteX;
                    xIndex++;
                    miny = System.Math.Min(numTilesInSpriteY, miny);
                    i++;
                }

                y += miny;
                yIndex++;
            }
        }
    }
}