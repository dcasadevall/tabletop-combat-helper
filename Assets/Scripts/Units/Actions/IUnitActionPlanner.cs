using UniRx.Async;

namespace Units.Actions {
    /// <summary>
    /// Implementations of this interface will handle planning an action, along with confirming or canceling such
    /// action.
    ///
    /// Currently, confirming / canceling an action is implementation specific.
    /// TODO: Maybe pass in the callbacks to check how an action should be confirmed / canceled instead.
    /// </summary>
    public interface IUnitActionPlanner {
        /// <summary>
        /// Plan the given action on the given unit, returning a task that will be completed once the action
        /// is either canceled or confirmed.
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        UniTask PlanAction(IUnit unit, UnitAction action);
    }
}