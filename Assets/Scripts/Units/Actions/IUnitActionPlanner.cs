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
        /// 
        ///  An <see cref="actionPlanObservable"/> is used to determine when the action has been planned.
        ///  </summary>
        ///  <param name="unit"></param>
        ///  <param name="action"></param>
        /// <param name="actionPlanObservable"></param>
        /// <returns></returns>
        UniTask PlanAction(IUnit unit,
                           UnitAction action,
                           IObservable<UnitActionPlanResult> actionPlanObservable);
    }
}