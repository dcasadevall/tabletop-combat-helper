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
        private readonly IGridUnitManager _gridUnitManager;
        private readonly ILogger _logger;
        private readonly List<IDisposable> _disposables;

        public UnitSelectionDetector(UnitMenuViewController unitMenuViewController,
                                     IUnitMovementController unitMovementController,
                                     IInputLock inputLock,
                                     IGridInputManager gridInputManager,
                                     IGridUnitManager gridUnitManager,
                                     ILogger logger) {
            _unitMenuViewController = unitMenuViewController;
            _unitMovementController = unitMovementController;
            _inputLock = inputLock;
            _gridInputManager = gridInputManager;
            _gridUnitManager = gridUnitManager;
            _logger = logger;
            _disposables = new List<IDisposable>();
        }

        public void Initialize() {
            var mouseDownStream = Observable.EveryUpdate()
                                            .Where(_ => Input.GetMouseButtonDown(0))
                                            .Where(_ => !_inputLock.IsLocked)
                                            .Where(_ => _gridInputManager.TileAtMousePosition != null)
                                            .Select(_ => new Tuple<string, IntVector2>("down",
                                                                                       // ReSharper disable once PossibleInvalidOperationException
                                                                                       _gridInputManager
                                                                                           .TileAtMousePosition
                                                                                           .Value));
            var mouseUpStream = Observable.EveryUpdate()
                                          .Where(_ => Input.GetMouseButtonUp(0))
                                          .Where(_ => !_inputLock.IsLocked)
                                          .Where(_ => _gridInputManager.TileAtMousePosition != null)
                                          .Select(_ => new Tuple<string, IntVector2>("up",
                                                                                     // ReSharper disable once PossibleInvalidOperationException
                                                                                     _gridInputManager
                                                                                         .TileAtMousePosition.Value));
            var clickStream = mouseDownStream.Merge(mouseUpStream)
                                             // Add Interval info from current to previous event
                                             .TimeInterval()
                                             // Emit current and previous values
                                             .Pairwise()
                                             // Grab only "mouse ups", preceded by mouse downs.
                                             .Where(x => x.Current.Value.Item1.Equals("up") &&
                                                         x.Previous.Value.Item1.Equals("down"))
                                             // And only those clicks which have been held and released within the same unit.
                                             .Where(x => x.Current.Value.Item2 == x.Previous.Value.Item2);

            // Short Click
            var shortClickStream = clickStream.Where(x => x.Current.Interval <= TimeSpan.FromMilliseconds(500));
            shortClickStream.Subscribe(next => OnMouseDown(next.Current.Value.Item2)).AddTo(_disposables);

            // Drag
            var mouseDragStream = Observable.EveryUpdate()
                                            .Where(_ => !_inputLock.IsLocked)
                                            .Where(_ => Input.GetMouseButton(0))
                                            .Where(_ => _gridInputManager.TileAtMousePosition.HasValue)
                                            .Select(_ => Input.mousePosition)
                                            .ThrottleFirst(TimeSpan.FromMilliseconds(100))
                                            .TimeInterval()
                                            .Pairwise()
                                            // This avoids long lapses between drags
                                            .Where(x => x.Current.Interval <= TimeSpan.FromMilliseconds(200))
                                            // Make sure we have moved at least 50 pixels
                                            .Where(x => Vector3.Distance(x.Current.Value, x.Previous.Value) > 50.0f)
                                            // ReSharper disable once PossibleInvalidOperationException
                                            .Select(_ => _gridInputManager
                                                         .TileAtMousePosition.Value);
            mouseDragStream.Subscribe(OnMouseHold).AddTo(_disposables);
        }

        public void Dispose() {
            _disposables.ForEach(x => x.Dispose());
            _disposables.Clear();
        }

        private void OnMouseDown(IntVector2 tileCoords) {
            IUnit[] units = _gridUnitManager.GetUnitsAtTile(tileCoords);
            if (units.Length == 0) {
                return;
            }

            // Input lock is handled by the VC since it owns other sub VCs, etc..
            // Ideally, we would lock here and await for the root menu to be closed.
            _unitMenuViewController.Show(units[0]);
        }

        private void OnMouseHold(IntVector2 tileCoords) {
            _logger.Log(LoggedFeature.Units, "{0}, {1}", tileCoords.x, tileCoords.y);
            
            IUnit[] units = _gridUnitManager.GetUnitsAtTile(tileCoords);
            if (units.Length == 0) {
                return;
            }

            if (_inputLock.IsLocked) {
                return;
            }

            // Input lock handled by unit movement controller
            _unitMovementController.DragAndDropUnit(units[0]);
        }
    }
}