using System;
using System.Linq;
using CameraSystem;
using Grid;
using UniRx;
using UniRx.Async;
using Units.Actions;
using UnityEngine;

namespace Units.Selection {
    internal class BatchUnitMenuViewController {
        private readonly UnitSelectionHighlighter _unitSelectionHighlighter;
        private readonly CameraInput _cameraInput;
        private readonly IUnitActionPlanner _unitActionPlanner;
        private readonly IGridInputManager _gridInputManager;

        public BatchUnitMenuViewController(UnitSelectionHighlighter unitSelectionHighlighter,
                                           CameraInput cameraInput,
                                           IUnitActionPlanner unitActionPlanner,
                                           IGridInputManager gridInputManager) {
            _unitSelectionHighlighter = unitSelectionHighlighter;
            _cameraInput = cameraInput;
            _unitActionPlanner = unitActionPlanner;
            _gridInputManager = gridInputManager;
        }

        /// <summary>
        /// Shows the batch selection UI and highlights the selected units.
        /// Returns a task that is completed once the UI has closed or the batch action is complete.
        /// </summary>
        /// <param name="units"></param>
        /// <returns></returns>
        public async UniTask ShowAndWaitForAction(IUnit[] units) {
            _unitSelectionHighlighter.HighlightUnits(units);

            // Mouse Up / Down streams
            var mouseUpStream = Observable.EveryUpdate()
                                          .Where(_ => Input.GetMouseButtonUp(0));
            var mouseDownStream = Observable.EveryUpdate()
                                            .Where(_ => Input.GetMouseButtonDown(0))
                                            .Where(_ => _gridInputManager.UnitsAtMousePosition.Length > 0)
                                            .Where(_ => units.Intersect(_gridInputManager.UnitsAtMousePosition).Any());

            // Await on an observable ends when the observable last emits a value.
            await mouseDownStream.Select(_ => mouseUpStream).First();
            // Now await on the batched action
            await _unitActionPlanner.PlanBatchedAction(units, UnitAction.DragAndDrop);

            _unitSelectionHighlighter.ClearHighlights();
        }

        private void Hide() { }
    }
}