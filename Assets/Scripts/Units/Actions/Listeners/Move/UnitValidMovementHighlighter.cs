using Grid;
using Grid.Highlighting;
using Grid.Positioning;
using Logging;
using Math;
using UnityEngine;
using ILogger = Logging.ILogger;

namespace Units.Actions.Listeners.Move {
    public class UnitValidMovementHighlighter : IUnitActionListener {
        private readonly IGridUnitManager _gridUnitManager;
        private readonly IGridPositionCalculator _gridPositionCalculator;
        private readonly IGridCellHighlightPool _gridCellHighlightPool;
        private readonly ILogger _logger;

        public UnitAction ActionType {
            get {
                return UnitAction.Move;
            }
        }

        public UnitValidMovementHighlighter(IGridUnitManager gridUnitManager,
                                            IGridPositionCalculator gridPositionCalculator, 
                                            IGridCellHighlightPool gridCellHighlightPool,
                                            ILogger logger) {
            _gridUnitManager = gridUnitManager;
            _gridPositionCalculator = gridPositionCalculator;
            _gridCellHighlightPool = gridCellHighlightPool;
            _logger = logger;
        }

        public void HandleActionPlanned(IUnit unit) {
            var coords = _gridUnitManager.GetUnitCoords(unit);
            if (coords == null) {
                _logger.LogError(LoggedFeature.Units, "Unit not in tile: {0}", unit);
                return;
            }

            var baseSpeedTiles = _gridPositionCalculator.GetTilesAtDistance(coords.Value, unit.UnitData.UnitStats.speed / 5);
            foreach (var tileCoords in baseSpeedTiles) {
                var worldPosition = _gridPositionCalculator.GetTileCenterWorldPosition(tileCoords);
                _gridCellHighlightPool.Spawn(worldPosition, new Color(0, 1, 0, 0.6f));
            }
        }

        public void Tick(IUnit unit) { }

        public void HandleActionConfirmed(IUnit unit, IntVector2 tileCoords) {
            _gridCellHighlightPool.DespawnAll();
        }

        public void HandleActionCanceled(IUnit unit) {
            _gridCellHighlightPool.DespawnAll();
        }
    }
}