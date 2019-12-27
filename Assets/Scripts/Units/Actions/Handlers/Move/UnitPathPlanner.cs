using System;
using Math;
using UniRx;

namespace Units.Actions.Handlers.Move {
    public class UnitPathPlanner : IUnitActionHandler {
        public UnitAction ActionType {
            get {
                return UnitAction.ChooseMovePath;
            }
        }

        public IObservable<IntVector2?> ConfirmActionObservable {
            get {
                return Observable.Never<IntVector2?>();
            }
        }

        public IObservable<UniRx.Unit> CancelActionObservable {
            get {
                return Observable.Never<UniRx.Unit>();
            }
        }

        public void HandleActionPlanned(IUnit unit) {
        }

        public void Tick(IUnit unit) {
        }

        public void HandleActionConfirmed(IUnit unit) {
        }

        public void HandleActionCanceled(IUnit unit) {
        }
    }
}