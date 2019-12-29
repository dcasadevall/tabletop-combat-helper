using System;
using Math;
using UniRx;

namespace Units.Actions.Handlers.Move {
    /// <summary>
    /// Action handler that shows UI previsualizing the path a unit will take upon confirming this action.
    /// Action is confirmed when the user clicks the "confirm" button.
    /// Action is canceled when the user clicks the "cancel" button.
    /// </summary>
    public class UnitPathPlanner : IUnitActionHandler {
        public UnitAction ActionType {
            get {
                return UnitAction.ChooseMovePath;
            }
        }

        public IObservable<UniRx.Unit> ConfirmActionObservable {
            get {
                return Observable.Return<UniRx.Unit>(UniRx.Unit.Default);
            }
        }

        public IObservable<UniRx.Unit> CancelActionObservable {
            get {
                return Observable.Never<UniRx.Unit>();
            }
        }

        public void HandleActionPlanned(IUnit unit) {
        }

        public void HandleActionConfirmed(IUnit unit) {
        }

        public void HandleActionCanceled(IUnit unit) {
        }
    }
}