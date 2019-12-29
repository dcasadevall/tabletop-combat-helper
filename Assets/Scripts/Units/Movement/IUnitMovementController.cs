using UniRx.Async;

namespace Units.Movement {
    public interface IUnitMovementController {
        /// <summary>
        /// Performs the process of moving a unit.
        /// This includes the UI to show valid movement destinations, path selection and movement animation.
        /// Task is considered complete once all steps are finished or the process is canceled at any point.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        UniTask<UniRx.Unit> PlanUnitMovement(IUnit unit);
        /// <summary>
        /// Starts the drag and drop process of a unit, which allows for the user to freely drag the unit
        /// around the grid.
        /// Task is considered complete once dragging finishes.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        UniTask<UniRx.Unit> DragAndDropUnit(IUnit unit);
    }
}