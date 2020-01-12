using System;
using Grid;
using UniRx;
using Units.Actions;

namespace Units.Movement.ActionHandlers {
    /// <summary>
    /// Action handler which shows the animation of a unit moving through a specific path.
    /// Action is confirmed once the unit reaches its destination.
    /// Action is never canceled.
    /// </summary>
    public class UnitMoveAnimator : ISingleUnitActionHandler {
        private readonly IGridUnitManager _gridUnitManager;
        private readonly IGridInputManager _gridInputManager;

        public UnitAction ActionType {
            get {
                return UnitAction.AnimateMovement;
            }
        }
        
        public IObservable<UniRx.Unit> ConfirmActionObservable {
            get {
                return Observable.Return(UniRx.Unit.Default);
            }
        }

        public IObservable<UniRx.Unit> CancelActionObservable {
            get {
                return Observable.Never<UniRx.Unit>();
            }
        }

        public UnitMoveAnimator(IGridUnitManager gridUnitManager, IGridInputManager gridInputManager) {
            _gridUnitManager = gridUnitManager;
            _gridInputManager = gridInputManager;
        }

        public void HandleActionPlanned(IUnit unit) {
            if (_gridInputManager.TileAtMousePosition.HasValue) {
                _gridUnitManager.PlaceUnitAtTile(unit, _gridInputManager.TileAtMousePosition.Value);
            }
        }

        public void HandleActionConfirmed(IUnit unit) {
        }

        public void HandleActionCanceled(IUnit unit) {
        }
    }
}