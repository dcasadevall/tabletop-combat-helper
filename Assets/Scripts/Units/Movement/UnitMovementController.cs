using System;
using InputSystem;
using Logging;
using UniRx.Async;
using Units.Actions;

namespace Units.Movement {
    public class UnitMovementController : IUnitMovementController {
        private readonly IInputLock _inputLock;
        private readonly IUnitActionPlanner _unitActionPlanner;
        private readonly ILogger _logger;

        public UnitMovementController(IInputLock inputLock, IUnitActionPlanner unitActionPlanner, ILogger logger) {
            _inputLock = inputLock;
            _unitActionPlanner = unitActionPlanner;
            _logger = logger;
        }

        public async UniTask<UniRx.Unit> PlanUnitMovement(IUnit unit) {
            using (_inputLock.Lock()) {
                // Action Planning / Confirmation
                _logger.Log(LoggedFeature.Units, "Planning Action: SelectMoveDestination");
                var destinationResult = await _unitActionPlanner.PlanAction(unit, UnitAction.SelectMoveDestination);
                _logger.Log(LoggedFeature.Units, "Done Planning Action: SelectMoveDestination");
                if (destinationResult.resultType == UnitActionPlanResult.PlanResultType.Canceled) {
                    await UniTask.DelayFrame(5);
                    return UniRx.Unit.Default;
                }

                _logger.Log(LoggedFeature.Units, "Planning Action: ChooseMovePath");
                var choosePathResult = await _unitActionPlanner.PlanAction(unit, UnitAction.ChooseMovePath);
                _logger.Log(LoggedFeature.Units, "Done Planning Action: ChooseMovePath");
                if (choosePathResult.resultType == UnitActionPlanResult.PlanResultType.Canceled) {
                    await UniTask.DelayFrame(5);
                    return UniRx.Unit.Default;
                }

                _logger.Log(LoggedFeature.Units, "Planning Action: AnimateMovement");
                await _unitActionPlanner.PlanAction(unit, UnitAction.AnimateMovement);
                _logger.Log(LoggedFeature.Units, "Done Planning Action: AnimateMovement");

                return UniRx.Unit.Default;
            }
        }

        public async UniTask<UniRx.Unit> DragAndDropUnit(IUnit unit) {
            using (_inputLock.Lock()) {
                await _unitActionPlanner.PlanAction(unit, UnitAction.DragAndDrop);
                
                return UniRx.Unit.Default;
            }
        }
    }
}