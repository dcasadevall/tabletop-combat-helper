using System;
using System.Linq;
using CameraSystem;
using CommandSystem;
using Grid;
using Grid.Commands;
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
        private readonly IGridInputManager _gridInputManager;
        private readonly IGridUnitManager _gridUnitManager;
        private readonly ICommandQueue _commandQueue;
        private readonly ILogger _logger;

        private IntVector2 _startCoords;
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

        public UnitDragAndDropHandler(IGridInputManager gridInputManager,
                                      IGridUnitManager gridUnitManager,
                                      ICommandQueue commandQueue,
                                      ILogger logger) {
            _gridInputManager = gridInputManager;
            _gridUnitManager = gridUnitManager;
            _commandQueue = commandQueue;
            _logger = logger;
        }

        public void HandleActionPlanned(IUnit unit) {
            _disposable =
                _gridInputManager.MouseEnteredTile.Pairwise().Subscribe(coordsPair => {
                    IntVector2 moveDistance = coordsPair.Current - coordsPair.Previous;
                    _commandQueue.Enqueue<MoveUnitCommand, MoveUnitData>(new MoveUnitData(unit.UnitId, moveDistance),
                                                                         CommandSource.Game);
                });
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
            
            _disposable = _gridInputManager.MouseEnteredTile.Pairwise().Subscribe(tileCoords => {
                foreach (var unit in units) {
                    IntVector2 moveDistance = tileCoords.Current - tileCoords.Previous;
                    _commandQueue.Enqueue<MoveUnitCommand, MoveUnitData>(new MoveUnitData(unit.UnitId, moveDistance),
                                                                         CommandSource.Game);
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