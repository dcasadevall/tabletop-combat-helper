using System;
using Grid;
using UniRx;
using Units.Actions;
using UnityEngine;

namespace Units.Movement.ActionHandlers {
    /// <summary>
    /// Action handler used to drag a unit around the grid, without movement restrictions.
    /// Action is confirmed when drag stops.
    /// Action is never canceled.
    /// </summary>
    public class UnitDragAndDropHandler : IUnitActionHandler {
        private readonly IGridInputManager _gridInputManager;
        private readonly IGridUnitManager _gridUnitManager;
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

        public UnitDragAndDropHandler(IGridInputManager gridInputManager, IGridUnitManager gridUnitManager) {
            _gridInputManager = gridInputManager;
            _gridUnitManager = gridUnitManager;
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
    }
}