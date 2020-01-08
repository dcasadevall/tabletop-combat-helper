using System;
using Grid;
using Grid.Positioning;
using UI.SelectionBox;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using ILogger = Logging.ILogger;

namespace Units.Selection {
    internal class BatchUnitSelectionDetector {
        private readonly EventSystem _eventSystem;
        private readonly ISelectionBox _selectionBox;
        private readonly IGridPositionCalculator _gridPositionCalculator;
        private readonly IGridUnitManager _gridUnitManager;
        private readonly ILogger _logger;

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
        /// is called. Will continue to receive selected units until <see cref="StopDetecting"/> is called.
        /// </summary>
        /// <returns></returns>
        public IObservable<IUnit[]> StartDetecting() {
            var observable = Observable.EveryUpdate()
                                       .Where(_ => Input.GetMouseButton(0))
                                       .Where(_ => !_eventSystem.IsPointerOverGameObject())
                                       .Zip(_selectionBox.Show(),
                                            (frame, rect) => _gridPositionCalculator.GetTilesCoveredByRect(rect))
                                       .Select(selectedTiles => _gridUnitManager.GetUnitsAtTiles(selectedTiles))
                                       .Where(units => units.Length > 0);
            return new SelectionObservable(observable, _selectionBox);
        }

        /// <summary>
        /// Class used to encapsulate the lifecycle of our <see cref="ISelectionBox"/> when used inside an observable.
        /// This allows users of <see cref="BatchUnitSelectionDetector"/> to only care about disposing the
        /// returned observable.
        /// </summary>
        private class SelectionObservable : IObservable<IUnit[]> {
            private readonly IObservable<IUnit[]> _observable;
            private readonly ISelectionBox _selectionBox;

            public SelectionObservable(IObservable<IUnit[]> observable, ISelectionBox selectionBox) {
                _observable = observable;
                _selectionBox = selectionBox;
            }

            public IDisposable Subscribe(IObserver<IUnit[]> observer) {
                return new SelectionObserver(_observable.Subscribe(observer), _selectionBox);
            }
        }

        /// <summary>
        /// Observer returned by the <see cref="SelectionObservable"/> when one subscribes to it.
        /// </summary>
        private class SelectionObserver : IDisposable {
            private readonly IDisposable _observer;
            private readonly ISelectionBox _selectionBox;

            public SelectionObserver(IDisposable observer, ISelectionBox selectionBox) {
                _observer = observer;
                _selectionBox = selectionBox;
            }

            public void Dispose() {
                _observer.Dispose();
                _selectionBox.Hide();
            }
        }
    }
}