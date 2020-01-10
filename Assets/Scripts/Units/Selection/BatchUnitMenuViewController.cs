using System;
using System.Linq;
using CameraSystem;
using Grid;
using UniRx;
using UniRx.Async;
using UnityEngine;

namespace Units.Selection {
    internal class BatchUnitMenuViewController {
        private readonly UnitSelectionHighlighter _unitSelectionHighlighter;
        private readonly CameraInput _cameraInput;
        private readonly IGridInputManager _gridInputManager;

        public BatchUnitMenuViewController(UnitSelectionHighlighter unitSelectionHighlighter,
                                           CameraInput cameraInput,
                                           IGridInputManager gridInputManager) {
            _unitSelectionHighlighter = unitSelectionHighlighter;
            _cameraInput = cameraInput;
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

//            var mouseDownStream = Observable.EveryUpdate()
//                                            .Where(_ => Input.GetMouseButtonDown(0))
//                                            .Where(_ => _gridInputManager.UnitsAtMousePosition.Length > 0)
//                                            .Where(_ => units.Intersect(_gridInputManager.UnitsAtMousePosition).Any());
//
//            var mouseUpStream = Observable.EveryUpdate()
//                                          .Where(_ => Input.GetMouseButtonUp(0))
//                                          .Select(_ => _cameraInput.MouseWorldPosition);
//
//            // Drag
//            // TODO: Detect initial emission with the proper unit, then propagate than.
//            // Right now, if we drag away from the tile, we don't know what unit we were dragging.
//            // Also, if we start dragging outside of the unit, then we can drag that unit as long as we hold the mouse
//            var mouseDragStream = Observable.Timer(TimeSpan.FromMilliseconds(300))
//                                            .TakeWhile(_ => Input.GetMouseButton(0))
//                                            .TakeWhile(_ => !_inputLock.IsLocked)
//                                            .TakeWhile(_ => _gridInputManager.UnitsAtMousePosition.Length > 0)
//                                            .TakeUntil(mouseUpStream)
//                                            .Select(_ => _gridInputManager.UnitsAtMousePosition)
//                                            .Repeat();


            _unitSelectionHighlighter.ClearHighlights();
        }

        private void Hide() { }
    }
}