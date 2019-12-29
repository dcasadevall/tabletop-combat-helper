using System;
using Grid;
using Math;
using UniRx;
using UnityEngine;

namespace Units.Actions.Handlers.Move {
    /// <summary>
    /// Action handler used to drag a unit around the grid, without movement restrictions.
    /// Action is confirmed when drag stops.
    /// Action is never canceled.
    /// </summary>
    public class UnitDragAndDropHandler : IUnitActionHandler {
        private readonly IGridInputManager _gridInputManager;

        public UnitAction ActionType {
            get {
                return UnitAction.FreeMove;
            }
        }

        public IObservable<UniRx.Unit> ConfirmActionObservable {
            get {
                return Observable.EveryUpdate()
                                 .Where(_ => Input.GetMouseButtonUp(0))
                                 .Select(_ => UniRx.Unit.Default);
            }
        }

        public IObservable<UniRx.Unit> CancelActionObservable {
            get {
                return Observable.Never<UniRx.Unit>();
            }
        }

        public UnitDragAndDropHandler(IGridInputManager gridInputManager) {
            _gridInputManager = gridInputManager;
        }

        public void HandleActionPlanned(IUnit unit) {
            throw new NotImplementedException();
        }

        public void Tick(IUnit unit) {
            throw new NotImplementedException();
        }

        public void HandleActionConfirmed(IUnit unit) {
            throw new NotImplementedException();
        }

        public void HandleActionCanceled(IUnit unit) {
            throw new NotImplementedException();
        }
    }
}