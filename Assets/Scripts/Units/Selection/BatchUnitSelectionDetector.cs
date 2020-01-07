using System;
using System.Threading;
using Grid;
using Grid.Positioning;
using Logging;
using Math;
using UI.SelectionBox;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.EventSystems;
using ILogger = Logging.ILogger;

namespace Units.Selection {
    public class BatchUnitSelectionDetector {
        private readonly EventSystem _eventSystem;
        private readonly ISelectionBox _selectionBox;
        private readonly IGridPositionCalculator _gridPositionCalculator;
        private readonly IGridUnitManager _gridUnitManager;
        private readonly ILogger _logger;
        private CancellationTokenSource _cancellationTokenSource;

        public BatchUnitSelectionDetector(EventSystem eventSystem, 
                                          ISelectionBox selectionBox,
                                          IGridPositionCalculator gridPositionCalculator,
                                          IGridUnitManager gridUnitManager,
                                          ILogger logger) {
            _eventSystem = eventSystem;
            _selectionBox = selectionBox;
            _gridPositionCalculator = gridPositionCalculator;
            _gridUnitManager = gridUnitManager;
            _logger = logger;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Starts detecting batch selections, and continues to do so until <see cref="StopDetecting"/>
        /// is called.
        /// </summary>
        /// <returns></returns>
        public IObservable<IUnit[]> DetectBatchSelections() {
            return DetectBatchSelectionAsync().ToObservable();
        }

        private async UniTask<IUnit[]> DetectBatchSelectionAsync() {
            UniTask.WaitUntil(() => {
                return false;
            }, cancellationToken: _cancellationTokenSource.Token);
            // Wait for a click, not on UI.
            await Observable.EveryUpdate()
                            .Where(_ => Input.GetMouseButton(0))
                            .Where(_ => !_eventSystem.IsPointerOverGameObject())
                            .TakeWhile(_ => !_cancellationTokenSource.IsCancellationRequested)
                            .FirstOrDefault();
                
            // Check how many units we selected.
            Rect selectionRect = await _selectionBox.Show();
            IntVector2[] tilesCoveredByRect = _gridPositionCalculator.GetTilesCoveredByRect(selectionRect);
            IUnit[] units = _gridUnitManager.GetUnitsAtTiles(tilesCoveredByRect);

            _logger.Log(LoggedFeature.Units, "Selected World Space: {0}", selectionRect);
            _logger.Log(LoggedFeature.Units, "Selected {0} Units", units.Length);
            return units;
        }
        
        /// <summary>
        /// Stops detecting batch selections. Does nothing unless <see cref="DetectBatchSelections"/> has been
        /// previously called.
        /// </summary>
        public void StopDetecting() {
            _cancellationTokenSource.Cancel();
        }
    }
}