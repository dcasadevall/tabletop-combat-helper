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
        /// Injected implementations of <see cref="ISingleUnitActionHandler"/> will be processed.
        ///  </summary>
        ///  <param name="unit"></param>
        ///  <param name="action"></param>
        /// <returns></returns>
        UniTask<UnitActionPlanResult> PlanAction(IUnit unit, UnitAction action);

        /// <summary>
        ///  Plan the given action on the given units, returning a task that will be completed once the action
        ///  is either canceled or confirmed.
        /// 
        /// Injected implementations of <see cref="IBatchedUnitActionHandler"/> will be processed.
        /// </summary>
        /// <param name="units"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        UniTask<UnitActionPlanResult> PlanBatchedAction(IUnit[] units, UnitAction action);
    }
}