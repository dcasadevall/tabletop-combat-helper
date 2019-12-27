using System;
using Math;
using UniRx;

namespace Units.Actions.Handlers.Move {
    public class UnitMoveAnimator : IUnitActionHandler {
        public UnitAction ActionType {
            get {
                return UnitAction.AnimateMovement;
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