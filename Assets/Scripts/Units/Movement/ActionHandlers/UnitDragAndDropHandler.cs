using System;
using System.Linq;
using CameraSystem;
using Grid;
using Logging;
using Math;
using UniRx;
using Units.Actions;
using UnityEngine;
using ILogger = Logging.ILogger;

namespace Units.Movement.ActionHandlers {
    /// <summary>
    /// Action handler used to drag a unit around the grid, without movement restrictions.
    /// Action is confirmed when drag stops.
    /// Action is never canceled.
    /// </summary>
    public class UnitDragAndDropHandler : ISingleUnitActionHandler, IBatchedUnitActionHandler {
        private readonly CameraInput _cameraInput;
        private readonly IGridInputManager _gridInputManager;
        private readonly IGridUnitManager _gridUnitManager;
        private readonly ILogger _logger;
        private IDisposable _disposable;

        public UnitAction ActionType {
            get {
                return UnitAction.DragAndDrop;
            }
        }

        public IObservable<UniRx.Unit> ConfirmActionObservable {
            get {
                return Observable.EveryUpdate()
                                 .Where(_ => !Input.GetMouseButton(0))
                                 .Select(_ => UniRx.Unit.Default);
            }
        }

        public IObservable<UniRx.Unit> CancelActionObservable {
            get {
                return Observable.Never<UniRx.Unit>();
            }
        }

        public UnitDragAndDropHandler(CameraInput cameraInput, 
                                      IGridInputManager gridInputManager,
                                      IGridUnitManager gridUnitManager,
                                      ILogger logger) {
            _cameraInput = cameraInput;
            _gridInputManager = gridInputManager;
            _gridUnitManager = gridUnitManager;
            _logger = logger;
        }

        public void HandleActionPlanned(IUnit unit) {
            _disposable =
                _gridInputManager.MouseEnteredTile.Subscribe(next => _gridUnitManager.PlaceUnitAtTile(unit, next));
        }

        public void HandleActionConfirmed(IUnit unit) {
            _disposable?.Dispose();
            _disposable = null;
        }

        public void HandleActionCanceled(IUnit unit) {
            _disposable?.Dispose();
            _disposable = null;
        }

        public void HandleActionPlanned(IUnit[] units) {
            if (_gridInputManager.TileAtMousePosition == null) {
                _logger.LogError(LoggedFeature.Units, "Drag and drop action planned on units when not inside grid.");
                return;
            }
            
            IntVector2 startCoordinate = _gridInputManager.TileAtMousePosition.Value;
            var unitStartingCoordinates =
                units.ToDictionary(unit => unit.UnitId, unit => _gridUnitManager.GetUnitCoords(unit).Value);
            _disposable = _gridInputManager.MouseEnteredTile.Subscribe(tileCoords => {
                foreach (var unit in units) {
                    IntVector2 startingUnitCoords = unitStartingCoordinates[unit.UnitId];
                    _gridUnitManager.PlaceUnitAtTile(unit, startingUnitCoords + tileCoords - startCoordinate);
                }
            });
        }

        public void HandleActionConfirmed(IUnit[] units) {
            _disposable?.Dispose();
            _disposable = null;
        }

        public void HandleActionCanceled(IUnit[] units) {
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}