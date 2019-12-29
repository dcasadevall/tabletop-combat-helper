using System;
using System.Collections.Generic;
using Grid;
using InputSystem;
using Math;
using UniRx;
using Units.Actions;
using Units.Spawning;
using UnityEngine;
using Zenject;
using ILogger = Logging.ILogger;

namespace Units.UI {
    public class UnitSelectionDetector : IInitializable, IDisposable {
        private readonly UnitMenuViewController _unitMenuViewController;
        private readonly IInputLock _inputLock;
        private readonly IGridInputManager _gridInputManager;
        private readonly IUnitRegistry _unitRegistry;
        private readonly IGridUnitManager _gridUnitManager;
        private readonly List<IDisposable> _disposables;

        public UnitSelectionDetector(UnitMenuViewController unitMenuViewController,
                                     IInputLock inputLock,
                                     IGridInputManager gridInputManager,
                                     IGridUnitManager gridUnitManager) {
            _unitMenuViewController = unitMenuViewController;
            _inputLock = inputLock;
            _gridInputManager = gridInputManager;
            _gridUnitManager = gridUnitManager;
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

            var longClickStream = clickStream.Where(x => x.Current.Interval > TimeSpan.FromMilliseconds(500));
            var shortClickStream = clickStream.Where(x => x.Current.Interval <= TimeSpan.FromMilliseconds(500));
            shortClickStream.Subscribe(next => OnMouseDown(next.Current.Value.Item2)).AddTo(_disposables);
            longClickStream.Subscribe(next => OnMouseHold(next.Current.Value.Item2)).AddTo(_disposables);
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

            _unitMenuViewController.Show(units[0]);
        }

        private void OnMouseHold(IntVector2 tileCoords) {
            // TODO: Trigger drag and drop script here
        }
    }
}