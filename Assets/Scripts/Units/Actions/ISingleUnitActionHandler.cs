namespace Units.Actions {
    public interface ISingleUnitActionHandler : IUnitActionHandler {
        /// <summary>
        /// Called before the action is executed.
        /// </summary>
        /// <param name="unit"></param>
        void HandleActionPlanned(IUnit unit);
        /// <summary>
        /// Called when the action has been confirmed (due to one of the <see cref="IUnitActionHandler.ConfirmActionObservable"/>
        /// being fired).
        /// </summary>
        /// <param name="unit"></param>
        void HandleActionConfirmed(IUnit unit);
        /// <summary>
        /// Called when the action has been canceled (due to one of the <see cref="IUnitActionHandler.CancelActionObservable"/>
        /// being fired).
        /// </summary>
        /// <param name="unit"></param>
        void HandleActionCanceled(IUnit unit);
    }
}