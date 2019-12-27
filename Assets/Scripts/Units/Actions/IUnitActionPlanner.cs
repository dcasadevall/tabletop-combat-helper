using System;
using UniRx.Async;

namespace Units.Actions {
    /// <summary>
    /// Implementations of this interface will handle planning an action, along with confirming or canceling such
    /// action.
    /// </summary>
    public interface IUnitActionPlanner {
        ///  <summary>
        ///  Plan the given action on the given unit, returning a task that will be completed once the action
        ///  is either canceled or confirmed.
        ///  </summary>
        ///  <param name="unit"></param>
        ///  <param name="action"></param>
        /// <returns></returns>
        UniTask<UnitActionPlanResult> PlanAction(IUnit unit, UnitAction action);
    }
}