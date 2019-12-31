using System;
using System.Collections.Generic;
using Grid;
using Grid.Highlighting;
using Grid.Positioning;
using Logging;
using Math;
using UniRx;
using Units.Actions;
using UnityEngine;
using ILogger = Logging.ILogger;

namespace Units.Movement.Handlers {
    /// <summary>
    /// This action handler shows all possible destinations for a unit to move to.
    /// Action is confirmed when the user taps or clicks on a valid destination.
    /// Action is canceled when the user right clicks anywhere, or clicks / taps outside of a valid destination.
    /// </summary>
    public class UnitDestinationSelector : IUnitActionHandler {
        private readonly IGridUnitManager _gridUnitManager;
        private readonly IGridInputManager _gridInputManager;
        private readonly IGridPositionCalculator _gridPositionCalculator;
        private readonly IGridCellHighlightPool _gridCellHighlightPool;
        private readonly ILogger _logger;
        private readonly HashSet<IntVector2> _validTiles;

        public UnitAction ActionType {
            get {
                return UnitAction.SelectMoveDestination;
            }
        }

        public IObservable<UniRx.Unit> ConfirmActionObservable {
            get {
                return Observable
                       .EveryUpdate().Where(_ => Input.GetMouseButton(0))
                       .Select(_ => _gridInputManager.TileAtMousePosition)
                       .Where(tile => tile != null && _validTiles.Contains(tile.Value))
                       .Select(_ => UniRx.Unit.Default);
            }
        }

        public IObservable<UniRx.Unit> CancelActionObservable {
            get {
                var rightClickStream = Observable
                                       .EveryUpdate()
                                       .Where(_ => Input.GetMouseButtonDown(1))
                                       .Select(_ => UniRx.Unit.Default);
                var leftClickOutsideStream = Observable
                                             .EveryUpdate().Where(_ => Input.GetMouseButton(0))
                                             .Select(_ => _gridInputManager.TileAtMousePosition)
                                             .Where(tile => tile != null && !_validTiles.Contains(tile.Value))
                                             .Select(_ => UniRx.Unit.Default);

                return leftClickOutsideStream.Merge(rightClickStream);
            }
        }

        public UnitDestinationSelector(IGridUnitManager gridUnitManager,
                                       IGridInputManager gridInputManager,
                                       IGridPositionCalculator gridPositionCalculator,
                                       IGridCellHighlightPool gridCellHighlightPool,
                                       ILogger logger) {
            _gridUnitManager = gridUnitManager;
            _gridInputManager = gridInputManager;
            _gridPositionCalculator = gridPositionCalculator;
            _gridCellHighlightPool = gridCellHighlightPool;
            _logger = logger;
            _validTiles = new HashSet<IntVector2>();
        }

        public void HandleActionPlanned(IUnit unit) {
            var coords = _gridUnitManager.GetUnitCoords(unit);
            if (coords == null) {
                _logger.LogError(LoggedFeature.Units, "Unit not in tile: {0}", unit);
                return;
            }

            var baseSpeedTiles =
                _gridPositionCalculator.GetTilesAtDistance(coords.Value, unit.UnitData.UnitStats.speed / 5);
            foreach (var tileCoords in baseSpeedTiles) {
                var worldPosition = _gridPositionCalculator.GetTileCenterWorldPosition(tileCoords);
                _gridCellHighlightPool.Spawn(worldPosition, new Color(0, 1, 0, 0.6f));
                _validTiles.Add(tileCoords);
            }
        }

        public void HandleActionConfirmed(IUnit unit) {
            _gridCellHighlightPool.DespawnAll();
        }

        public void HandleActionCanceled(IUnit unit) {
            _gridCellHighlightPool.DespawnAll();
        }
    }
}