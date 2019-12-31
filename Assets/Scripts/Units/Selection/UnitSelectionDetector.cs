using System;
using System.Collections.Generic;
using System.Diagnostics;
using Grid;
using InputSystem;
using Logging;
using Math;
using UniRx;
using Units.Actions;
using Units.Movement;
using Units.Selection;
using Units.Spawning;
using UnityEngine;
using Zenject;
using ILogger = Logging.ILogger;

namespace Units.UI {
    public class UnitSelectionDetector : IInitializable, IDisposable {
        private readonly UnitMenuViewController _unitMenuViewController;
        private readonly IUnitMovementController _unitMovementController;
        private readonly IInputLock _inputLock;
        private readonly IGridInputManager _gridInputManager;
        private readonly IUnitRegistry _unitRegistry;
        private readonly List<IDisposable> _disposables;

        public UnitSelectionDetector(UnitMenuViewController unitMenuViewController,
                                     IUnitMovementController unitMovementController,
                                     IInputLock inputLock,
                                     IGridInputManager gridInputManager) {
            _unitMenuViewController = unitMenuViewController;
            _unitMovementController = unitMovementController;
            _inputLock = inputLock;
            _gridInputManager = gridInputManager;
            _disposables = new List<IDisposable>();
        }

        public void Initialize() {
            var mouseDownStream = Observable.EveryUpdate()
                                            .Where(_ => Input.GetMouseButtonDown(0))
                                            .Where(_ => !_inputLock.IsLocked)
                                            .Where(_ => _gridInputManager.UnitsAtMousePosition.Length > 0)
                                            .Select(_ => new Tuple<string, IUnit[]>("down",
                                                                                    _gridInputManager
                                                                                        .UnitsAtMousePosition));
            var mouseUpStream = Observable.EveryUpdate()
                                          .Where(_ => Input.GetMouseButtonUp(0))
                                          .Where(_ => !_inputLock.IsLocked)
                                          .Where(_ => _gridInputManager.UnitsAtMousePosition.Length > 0)
                                          .Select(_ => new Tuple<string, IUnit[]>("up",
                                                                                  _gridInputManager
                                                                                      .UnitsAtMousePosition));
            var clickStream = mouseDownStream.Merge(mouseUpStream)
                                             // Add Interval info from current to previous event
                                             .TimeInterval()
                                             // Emit current and previous values
                                             .Pairwise()
                                             // Grab only "mouse ups", preceded by mouse downs.
                                             .Where(x => x.Current.Value.Item1.Equals("up") &&
                                                         x.Previous.Value.Item1.Equals("down"));

            // Short Click
            var shortClickStream = clickStream.Where(x => x.Current.Interval <= TimeSpan.FromMilliseconds(300));
            shortClickStream.Subscribe(next => OnMouseDown(next.Current.Value.Item2)).AddTo(_disposables);

            // Drag
            // TODO: Detect initial emission with the proper unit, then propagate than.
            // Right now, if we drag away from the tile, we don't know what unit we were dragging.
            // Also, if we start dragging outside of the unit, then we can drag that unit as long as we hold the mouse
            var mouseDragStream = Observable.Timer(TimeSpan.FromMilliseconds(300))
                                            .TakeWhile(_ => Input.GetMouseButton(0))
                                            .TakeWhile(_ => !_inputLock.IsLocked)
                                            .TakeWhile(_ => _gridInputManager.UnitsAtMousePosition.Length > 0)
                                            .TakeUntil(mouseUpStream)
                                            .Select(_ => _gridInputManager.UnitsAtMousePosition)
                                            .Repeat();

            mouseDragStream.Subscribe(OnMouseDrag).AddTo(_disposables);
        }

        public void Dispose() {
            _disposables.ForEach(x => x.Dispose());
            _disposables.Clear();
        }

        private void OnMouseDown(IUnit[] units) {
            // Input lock is handled by the VC since it owns other sub VCs, etc..
            // Ideally, we would lock here and await for the root menu to be closed.
            _unitMenuViewController.Show(units[0]);
        }

        private void OnMouseDrag(IUnit[] units) {
            // Input lock handled by unit movement controller
            _unitMovementController.DragAndDropUnit(units[0]);
        }
    }
}