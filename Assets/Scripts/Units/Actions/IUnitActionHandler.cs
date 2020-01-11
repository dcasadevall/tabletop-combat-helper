using System;
using Math;

namespace Units.Actions {
    /// <summary>
    /// Smaller unit of logic that handles different side effects of planning and performing an action
    /// i.e: Visuals of moving a unit / previewing where it will go, etc..
    /// </summary>
    public interface IUnitActionHandler {
        /// <summary>
        /// Type of action this handler belongs to. All handlers of the same <see cref="UnitAction"/> type will be
        /// processed serially, without a deterministic order.
        /// </summary>
        UnitAction ActionType { get; }
        /// <summary>
        /// An action will be confirmed if ANY of the <see cref="IUnitActionHandler.ConfirmActionObservable"/>s is fired.
        /// </summary>
        IObservable<UniRx.Unit> ConfirmActionObservable { get; }
        /// <summary>
        /// An action will be canceled if ANY of the <see cref="IUnitActionHandler.CancelActionObservable"/>s is fired.
        /// </summary>
        IObservable<UniRx.Unit> CancelActionObservable { get; }
    }
}