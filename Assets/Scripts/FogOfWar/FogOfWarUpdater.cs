using System;
using System.Collections.Generic;
using Grid;
using Grid.Positioning;
using Math;
using Units;
using Zenject;

namespace FogOfWar {
    public class FogOfWarUpdater : IInitializable, IDisposable {
        private readonly IGrid _grid;
        private readonly IGridUnitManager _gridUnitManager;
        private readonly IGridPositionCalculator _gridPositionCalculator;
        private readonly Dictionary<IntVector2, TileVisibilityData> _visibilityMatrix;

        public FogOfWarUpdater(IGrid grid, IGridUnitManager gridUnitManager,
                               IGridPositionCalculator gridPositionCalculator) {
            _grid = grid;
            _gridUnitManager = gridUnitManager;
            _gridPositionCalculator = gridPositionCalculator;
            _visibilityMatrix = new Dictionary<IntVector2, TileVisibilityData>();
        }

        public void Initialize() {
            for (int x = 0; x < _grid.NumTilesX; x++) {
                for (int y = 0; y < _grid.NumTilesY; y++) {
                    _visibilityMatrix[IntVector2.Of(x, y)] = new TileVisibilityData();
                }
            }
            
            _gridUnitManager.UnitPlacedAtTile += HandleUnitPlacedAtTile;
            _gridUnitManager.UnitRemovedFromTile += HandleUnitRemovedFromTile;
        }

        public void Dispose() {
            _gridUnitManager.UnitPlacedAtTile -= HandleUnitPlacedAtTile;
            _gridUnitManager.UnitRemovedFromTile -= HandleUnitRemovedFromTile;
        }

        private void HandleUnitPlacedAtTile(IUnit unit, IntVector2 tileCoords) {
            IntVector2[] visibileTiles =
                _gridPositionCalculator.GetTilesAtDistance(tileCoords, unit.UnitData.UnitStats.visibilityRadius);

            foreach (var coords in visibileTiles) {
                _visibilityMatrix[coords].tileVisibilityType = TileVisibilityType.Visible;
                _visibilityMatrix[coords].lightSourceCount++;
            }
        }

        private void HandleUnitRemovedFromTile(IUnit unit, IntVector2 tileCoords) {
            IntVector2[] visibileTiles =
                _gridPositionCalculator.GetTilesAtDistance(tileCoords, unit.UnitData.UnitStats.visibilityRadius);

            foreach (var coords in visibileTiles) {
                _visibilityMatrix[coords].lightSourceCount--;
                if (_visibilityMatrix[coords].lightSourceCount == 0) {
                    _visibilityMatrix[coords].tileVisibilityType = TileVisibilityType.VisitedFog;
                }
            }
        }

        private class TileVisibilityData {
            /// <summary>
            /// The amount of sources that are making this tile visible (if any).
            /// </summary>
            public int lightSourceCount;
            public TileVisibilityType tileVisibilityType;

            public TileVisibilityData() {
                tileVisibilityType = TileVisibilityType.NotVisitedFog;
            }
        }
    }
}