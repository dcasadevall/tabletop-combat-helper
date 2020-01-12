using System;

namespace Units.Actions {
    public interface IBatchedUnitActionHandler : IUnitActionHandler {
        /// <summary>
        /// Called before the action is executed.
        /// </summary>
        /// <param name="units"></param>
        void HandleActionPlanned(IUnit[] units);
        /// <summary>
        /// Called when the action has been confirmed (due to one of the <see cref="IUnitActionHandler.ConfirmActionObservable"/>
        /// being fired).
        /// </summary>
        /// <param name="units"></param>
        void HandleActionConfirmed(IUnit[] units);
        /// <summary>
        /// Called when the action has been canceled (due to one of the <see cref="IUnitActionHandler.CancelActionObservable"/>
        /// being fired).
        /// </summary>
        /// <param name="units"></param>
        void HandleActionCanceled(IUnit[] units);
    }
}