
using Math;

namespace Units.Actions {
    public sealed class UnitActionPlanResult {
        public enum PlanResultType {
            None,
            Confirmed,
            Canceled
        }

        public PlanResultType resultType;

        public static UnitActionPlanResult MakeConfirmed() {
            return new UnitActionPlanResult(PlanResultType.Confirmed);
        }

        public static UnitActionPlanResult MakeCanceled() {
            return new UnitActionPlanResult(PlanResultType.Canceled);
        }
        
        private UnitActionPlanResult(PlanResultType resultType) {
            this.resultType = resultType;
        }
    }
}