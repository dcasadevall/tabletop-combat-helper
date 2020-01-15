using System;
using System.Collections.Generic;
using Grid;
using Grid.Positioning;
using Math;
using Units;
using Zenject;

namespace TileVisibility {
    /// <summary>
    /// Class responsible for updating tile visibility based on unit movement.
    /// It will notify injected <see cref="ITileVisibilityDelegate"/>s of tile visibility changes.
    /// </summary>
    public class TileVisibilityUpdater : IInitializable, IDisposable {
        private readonly List<ITileVisibilityDelegate> _tileVisibilityDelegates;
        private readonly IGrid _grid;
        private readonly IGridUnitManager _gridUnitManager;
        private readonly IGridPositionCalculator _gridPositionCalculator;
        private readonly Dictionary<IntVector2, TileVisibilityData> _visibilityMatrix;

        public TileVisibilityUpdater(List<ITileVisibilityDelegate> tileVisibilityDelegates,
                                     IGrid grid,
                                     IGridUnitManager gridUnitManager,
                                     IGridPositionCalculator gridPositionCalculator) {
            _tileVisibilityDelegates = tileVisibilityDelegates;
            _grid = grid;
            _gridUnitManager = gridUnitManager;
            _gridPositionCalculator = gridPositionCalculator;
            _visibilityMatrix = new Dictionary<IntVector2, TileVisibilityData>();
        }

        public void Initialize() {
            for (int x = 0; x < _grid.NumTilesX; x++) {
                for (int y = 0; y < _grid.NumTilesY; y++) {
                    var coords = IntVector2.Of(x, y);
                    _visibilityMatrix[coords] = new TileVisibilityData();
                    _tileVisibilityDelegates.ForEach(del => del.HandleTileVisibilityChanged(coords,
                                                                                            _visibilityMatrix[coords]
                                                                                                .tileVisibilityType));
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
                _tileVisibilityDelegates.ForEach(del => del.HandleTileVisibilityChanged(coords,
                                                                                        _visibilityMatrix[coords]
                                                                                            .tileVisibilityType));
                _visibilityMatrix[coords].lightSourceCount++;
            }
        }

        private void HandleUnitRemovedFromTile(IUnit unit, IntVector2 tileCoords) {
            IntVector2[] visibileTiles =
                _gridPositionCalculator.GetTilesAtDistance(tileCoords, unit.UnitData.UnitStats.visibilityRadius);

            foreach (var coords in visibileTiles) {
                _visibilityMatrix[coords].lightSourceCount--;
                if (_visibilityMatrix[coords].lightSourceCount == 0) {
                    _visibilityMatrix[coords].tileVisibilityType = TileVisibilityType.VisitedNotInSight;
                    _tileVisibilityDelegates.ForEach(del => del.HandleTileVisibilityChanged(coords,
                                                                                            _visibilityMatrix[coords]
                                                                                                .tileVisibilityType));
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
                tileVisibilityType = TileVisibilityType.NotVisited;
            }
        }
    }
}