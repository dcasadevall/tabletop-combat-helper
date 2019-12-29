using System;
using System.Collections.Generic;
using System.Linq;
using Logging;
using UniRx;
using UniRx.Async;

namespace Units.Actions {
    /// <summary>
    /// Implementation of IUnitActionPlanner which broadcasts the different action states to its listeners.
    /// This allows us to separate responsibilities in smaller components that handle different side effects
    /// of performing unit actions.
    /// </summary>
    public class UnitActionBroadcaster : IUnitActionPlanner {
        private readonly List<IUnitActionHandler> _actionHandlers;
        private readonly ILogger _logger;
        private readonly List<IDisposable> _disposables;

        UnitActionBroadcaster(List<IUnitActionHandler> actionHandlers, ILogger logger) {
            _actionHandlers = actionHandlers;
            _logger = logger;
            _disposables = new List<IDisposable>();
        }

        public async UniTask<UnitActionPlanResult> PlanAction(IUnit unit, UnitAction action) {
            // Action Plan Event
            HandleActionPlanned(unit, action);
            
            // Subscribe to confirm / cancel. They will fire before Tick if possible.
            UnitActionPlanResult result = null;
            var confirmObservables =
                _actionHandlers.Where(x => x.ActionType == action).Select(x => x.ConfirmActionObservable);
            confirmObservables.Merge().Subscribe(coords => {
                HandleActionConfirmed(unit, action);
                result = UnitActionPlanResult.MakeConfirmed();
            }).AddTo(_disposables);

            var cancelObservables =
                _actionHandlers.Where(x => x.ActionType == action).Select(x => x.CancelActionObservable);
            cancelObservables.Merge().Subscribe(coords => {
                HandleActionCanceled(unit, action);
                result = UnitActionPlanResult.MakeCanceled();
            }).AddTo(_disposables);
            
            // Wait until result is received
            await UniTask.WaitUntil(() => result != null);
            return result;
        }

        private void HandleActionPlanned(IUnit unit, UnitAction action) {
            _logger.Log(LoggedFeature.Units, $"Action: {action} planned on unit: {unit.UnitData.Name}");
            _actionHandlers.Where(x => x.ActionType == action).ToList().ForEach(x => x.HandleActionPlanned(unit));
        }

        private void HandleActionTick(IUnit unit, UnitAction unitAction) {
            _actionHandlers.Where(x => x.ActionType == unitAction).ToList().ForEach(x => x.Tick(unit));
        }

        private void HandleActionConfirmed(IUnit unit, UnitAction action) {
            _logger.Log(LoggedFeature.Units,
                        $"Action: {action} confirmed on unit: {unit.UnitData.Name}.");
            _actionHandlers.Where(x => x.ActionType == action).ToList()
                            .ForEach(x => x.HandleActionConfirmed(unit));

            _disposables.ForEach(x => x.Dispose());
            _disposables.Clear();
        }

        private void HandleActionCanceled(IUnit unit, UnitAction action) {
            _logger.Log(LoggedFeature.Units, $"Action: {action} canceled on unit: {unit}");
            _actionHandlers.Where(x => x.ActionType == action).ToList().ForEach(x => x.HandleActionCanceled(unit));

            _disposables.ForEach(x => x.Dispose());
            _disposables.Clear();
        }

        private void Tick(IUnit unit, UnitAction unitAction) {
            HandleActionTick(unit, unitAction);
        }
    }
}