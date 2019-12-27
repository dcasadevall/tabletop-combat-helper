using Math;

namespace Units.Actions {
    public sealed class UnitActionPlanResult {
        public enum PlanResultType {
            Confirmed,
            Canceled
        }

        public readonly PlanResultType resultType;
        public readonly IntVector2 tileCoords;
        
        public static UnitActionPlanResult MakeConfirmed(IntVector2 tileCoords) {
            return new UnitActionPlanResult(PlanResultType.Confirmed, tileCoords);
        }

        public static UnitActionPlanResult MakeCanceled() {
            return new UnitActionPlanResult(PlanResultType.Canceled);
        }

        private UnitActionPlanResult(PlanResultType resultType) {
            this.resultType = resultType;
        }
        
        private UnitActionPlanResult(PlanResultType resultType, IntVector2 tileCoords) {
            this.resultType = resultType;
            this.tileCoords = tileCoords;
        }
    }
}