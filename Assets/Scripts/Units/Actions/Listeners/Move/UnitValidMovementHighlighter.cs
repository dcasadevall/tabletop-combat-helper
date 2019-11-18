using Math;

namespace Units.Actions.Listeners.Move {
    public class UnitValidMovementHighlighter : IUnitActionListener {
        public UnitAction ActionType {
            get {
                return UnitAction.Move;
            }
        }
        
        public void HandleActionPlanned(IUnit unit) {
        }

        public void Tick(IUnit unit) {
        }

        public void HandleActionConfirmed(IUnit unit, IntVector2 tileCoords) {
        }

        public void HandleActionCanceled(IUnit unit) {
        }
    }
}