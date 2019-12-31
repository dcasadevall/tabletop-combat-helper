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
            var lockId = _inputLock.Lock();
            if (lockId == null) {
                var msg = "Could not acquire input lock";
                _logger.LogError(LoggedFeature.Units, msg);
                throw new Exception(msg);
            }

            // Action Planning / Confirmation
            _logger.Log(LoggedFeature.Units, "Planning Action: SelectMoveDestination");
            var destinationResult = await _unitActionPlanner.PlanAction(unit, UnitAction.SelectMoveDestination);
            _logger.Log(LoggedFeature.Units, "Done Planning Action: SelectMoveDestination");
            if (destinationResult.resultType == UnitActionPlanResult.PlanResultType.Canceled) {
                await UniTask.DelayFrame(5);
                _inputLock.Unlock(lockId.Value);
                return UniRx.Unit.Default;
            }
            
            _logger.Log(LoggedFeature.Units, "Planning Action: ChooseMovePath");
            var choosePathResult = await _unitActionPlanner.PlanAction(unit, UnitAction.ChooseMovePath);
            _logger.Log(LoggedFeature.Units, "Done Planning Action: ChooseMovePath");
            if (choosePathResult.resultType == UnitActionPlanResult.PlanResultType.Canceled) {
                await UniTask.DelayFrame(5);
                _inputLock.Unlock(lockId.Value);
                return UniRx.Unit.Default;
            }
            
            _logger.Log(LoggedFeature.Units, "Planning Action: AnimateMovement");
            await _unitActionPlanner.PlanAction(unit, UnitAction.AnimateMovement);
            _logger.Log(LoggedFeature.Units, "Done Planning Action: AnimateMovement");
            
            // Release input lock delay to avoid input conflicts
            _inputLock.Unlock(lockId.Value);
            return UniRx.Unit.Default;
        }

        public async UniTask<UniRx.Unit> DragAndDropUnit(IUnit unit) {
            var lockId = _inputLock.Lock();
            if (lockId == null) {
                var msg = "Failed to acquire input lock";
                _logger.LogError(LoggedFeature.Units, msg);
                throw new Exception(msg);
            }
            
            await _unitActionPlanner.PlanAction(unit, UnitAction.DragAndDrop);
            
            _inputLock.Unlock(lockId.Value);
            return UniRx.Unit.Default;
        }
    }
}