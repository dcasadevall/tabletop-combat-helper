using System.Collections.Generic;
using Grid;
using Grid.Positioning;
using Logging;
using Map;
using Math;
using UnityEngine;
using Zenject;
using ILogger = Logging.ILogger;

namespace Drawing.DrawableTiles {
    public class DrawableTileRegistry : IDrawableTileRegistry {
        private Dictionary<IntVector2, IDrawableTile> _tiles = new Dictionary<IntVector2, IDrawableTile>();

        private readonly ILogger _logger;
        private readonly IGrid _grid;
        private readonly IGridPositionCalculator _gridPositionCalculator;
        private readonly Sprite _drawableSprite;
        private readonly IMapSectionData _mapSectionData;
        private readonly DrawableTileBehaviour.Pool _drawableTilePool;

        public DrawableTileRegistry(ILogger logger,
                                    IGrid grid,
                                    IGridPositionCalculator gridPositionCalculator,
                                    IFactory<int, Sprite> drawableSpriteFactory,
                                    IMapSectionData mapSectionData,
                                    DrawableTileBehaviour.Pool drawableTilePool) {
            _logger = logger;
            _grid = grid;
            _gridPositionCalculator = gridPositionCalculator;
            _drawableSprite = drawableSpriteFactory.Create(0);
            _mapSectionData = mapSectionData;
            _drawableTilePool = drawableTilePool;
        }

        public IDrawableTile GetDrawableTileAtCoordinates(IntVector2 tileCoords) {
            if (!_grid.TileBounds().Contains(tileCoords)) {
                _logger.LogError(LoggedFeature.Drawing,
                                 "GetDrawableTileAtCoordinates called out of bounds. {0}",
                                 tileCoords);
                return null;
            }
            
            IntVector2 drawableCoords = GetDrawableTilePositionForTile(tileCoords);
            if (_tiles.ContainsKey(drawableCoords)) {
                return _tiles[drawableCoords];
            }

            IntVector2 bottomLeftTileCoords = GetBottomLeftGridTileCoords(tileCoords);
            Vector2 tileOrigin = _gridPositionCalculator.GetTileOriginWorldPosition(bottomLeftTileCoords);
            Vector2 tileCenter = GetCenterWorldPositionWithOrigin(tileOrigin);
            
            _tiles[drawableCoords] = _drawableTilePool.Spawn(tileCenter);
            return _tiles[drawableCoords];
        }

        public IEnumerable<IDrawableTile> GetAllTiles() {
            return _tiles.Values;
        }

        public Vector2? GetLocalPosition(Vector2 worldPosition) {
            IntVector2? tileCoords = _gridPositionCalculator.GetTileContainingWorldPosition(worldPosition);
            if (tileCoords == null) {
                _logger.Log(LoggedFeature.Drawing, "TileCoords not found for world position: {0}", worldPosition);
                return null;
            }

            IntVector2 bottomLeftTileCoords = GetBottomLeftGridTileCoords(tileCoords.Value);
            Vector2 tileOrigin = _gridPositionCalculator.GetTileOriginWorldPosition(bottomLeftTileCoords);
            return worldPosition - tileOrigin;
        }

        IntVector2 GetDrawableTilePositionForTile(IntVector2 tileCoords) {
            int xSize = Mathf.CeilToInt(_drawableSprite.bounds.size.x / _mapSectionData.PixelsPerUnit);
            int ySize = Mathf.CeilToInt(_drawableSprite.bounds.size.y / _mapSectionData.PixelsPerUnit);

            return IntVector2.Of(tileCoords.x / xSize, tileCoords.y / ySize);
        }

        IntVector2 GetBottomLeftGridTileCoords(IntVector2 tileCoords) {
            int xSize = Mathf.CeilToInt(_drawableSprite.bounds.size.x / _mapSectionData.PixelsPerUnit);
            int ySize = Mathf.CeilToInt(_drawableSprite.bounds.size.y / _mapSectionData.PixelsPerUnit);
            
            return IntVector2.Of((tileCoords.x / xSize) * xSize, (tileCoords.y / ySize) * ySize);
        }

        Vector2 GetCenterWorldPositionWithOrigin(Vector2 drawableTileOrigin) {
            int xSize = Mathf.CeilToInt(_drawableSprite.bounds.size.x / _mapSectionData.PixelsPerUnit);
            int ySize = Mathf.CeilToInt(_drawableSprite.bounds.size.y / _mapSectionData.PixelsPerUnit);
            
            return new Vector2(drawableTileOrigin.x + xSize / 2.0f, drawableTileOrigin.y + ySize / 2.0f);
        }
    }
}