using System;
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
    public class UnitSelectionDetector : IInitializable {
        private readonly UnitMenuViewController _unitMenuViewController;
        private readonly IInputLock _inputLock;
        private readonly IGridInputManager _gridInputManager;
        private readonly IUnitRegistry _unitRegistry;
        private readonly IGridUnitManager _gridUnitManager;

        public UnitSelectionDetector(UnitMenuViewController unitMenuViewController,
                                     IInputLock inputLock,
                                     IGridInputManager gridInputManager,
                                     IGridUnitManager gridUnitManager) {
            _unitMenuViewController = unitMenuViewController;
            _inputLock = inputLock;
            _gridInputManager = gridInputManager;
            _gridUnitManager = gridUnitManager;
        }

        public void Initialize() {
            var mouseDownStream = Observable.EveryUpdate()
                                            .Where(_ => Input.GetMouseButtonDown(0))
                                            .Where(_ => _gridInputManager.TileAtMousePosition != null)
                                            .Select(_ => new Tuple<string, IntVector2>("down",
                                                                                       _gridInputManager
                                                                                           .TileAtMousePosition
                                                                                           .Value));

            var mouseUpStream = Observable.EveryUpdate()
                                          .Where(_ => Input.GetMouseButtonUp(0))
                                          .Where(_ => _gridInputManager.TileAtMousePosition != null)
                                          .Select(_ => new Tuple<string, IntVector2>("up",
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
            
            mouseDownStream.Merge(mouseUpStream.Take(TimeSpan.FromMilliseconds(250)));
            mouseDownStream.Merge(Observable.Timer(TimeSpan.FromMilliseconds(1000)).TakeUntil(mouseUpStream)
                                            .Select(_ => _gridInputManager.TileAtMousePosition.Value));

//            var clicks = downs.flatMapLatest(function(){
//                return ups.takeUntil(Rx.Observable.timer(250));
//            });
//
//            var longDownsStart = downs.flatMapLatest(function(){
//                return Rx.Observable.timer(1000).takeUntil(ups);
//            }); 
        }

        public void Tick() {
            if (_inputLock.IsLocked) {
                return;
            }

            if (Input.GetMouseButtonDown(0)) {
                OnMouseDown();
            }
        }

        private void OnMouseDown() {
            IntVector2? tileCoords = _gridInputManager.TileAtMousePosition;
            if (tileCoords == null) {
                return;
            }

            IUnit[] units = _gridUnitManager.GetUnitsAtTile(tileCoords.Value);
            if (units.Length == 0) {
                return;
            }

            _unitMenuViewController.Show(units[0]);
        }
    }
}