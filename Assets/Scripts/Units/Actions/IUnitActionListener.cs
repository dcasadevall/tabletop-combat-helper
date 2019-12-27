using Math;

namespace Units.Actions {
    /// <summary>
    /// Smaller unit of logic that handles different side effects of planning and performing an action
    /// i.e: Visuals of moving a unit / previewing where it will go, etc..
    /// </summary>
    public interface IUnitActionListener {
        UnitAction ActionType { get; }
        void HandleActionPlanned(IUnit unit);
        void Tick(IUnit unit);
        void HandleActionConfirmed(IUnit unit);
        void HandleActionCanceled(IUnit unit);
    }
}