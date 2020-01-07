using System;
using Grid;
using Grid.Positioning;
using UI.SelectionBox;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Units.Selection {
    public class BatchUnitSelectionDetector {
        private readonly EventSystem _eventSystem;
        private readonly ISelectionBox _selectionBox;
        private readonly IGridPositionCalculator _gridPositionCalculator;
        private readonly IGridUnitManager _gridUnitManager;

        public BatchUnitSelectionDetector(EventSystem eventSystem,
                                          ISelectionBox selectionBox,
                                          IGridPositionCalculator gridPositionCalculator,
                                          IGridUnitManager gridUnitManager) {
            _eventSystem = eventSystem;
            _selectionBox = selectionBox;
            _gridPositionCalculator = gridPositionCalculator;
            _gridUnitManager = gridUnitManager;
        }

        /// <summary>
        /// Returns an observable which receives a stream of units selected via <see cref="ISelectionBox"/>.
        /// is called.
        /// </summary>
        /// <returns></returns>
        public UniTask<IUnit[]> GetSelectedUnits() {
            return Observable.EveryUpdate()
                             .Where(_ => Input.GetMouseButton(0))
                             .Where(_ => !_eventSystem.IsPointerOverGameObject())
                             .TakeUntil()
                             .ZipLatest(_selectionBox.Show().ToObservable(),
                                        (frame, rect) => _gridPositionCalculator.GetTilesCoveredByRect(rect))
                             .Select(selectedTiles => _gridUnitManager.GetUnitsAtTiles(selectedTiles))
                             .Where(units => units.Length > 0);
        }
    }
}