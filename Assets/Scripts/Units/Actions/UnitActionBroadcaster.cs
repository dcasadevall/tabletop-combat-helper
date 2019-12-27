using System;
using System.Collections.Generic;
using System.Linq;
using Grid;
using Logging;
using Math;
using UniRx;
using UniRx.Async;
using UnityEngine;
using ILogger = Logging.ILogger;

namespace Units.Actions {
    /// <summary>
    /// Implementation of IUnitActionPlanner which broadcasts the different action states to its listeners.
    /// This allows us to separate responsibilities in smaller components that handle different side effects
    /// of performing unit actions.
    /// </summary>
    public class UnitActionBroadcaster : IUnitActionPlanner {
        private readonly List<IUnitActionListener> _actionListeners;
        private readonly ILogger _logger;
        private IDisposable _disposable;
        private IDisposable _planCompleteObserver;

        UnitActionBroadcaster(List<IUnitActionListener> actionListeners, ILogger logger) {
            _actionListeners = actionListeners;
            _logger = logger;
        }

        public UniTask PlanAction(IUnit unit,
                                  UnitAction action,
                                  IObservable<UnitActionPlanResult> actionPlanObservable) {
            _planCompleteObserver = actionPlanObservable.Subscribe(result => {
                if (result == UnitActionPlanResult.Confirmed) {
                    HandleActionConfirmed(unit, action);
                } else {
                    HandleActionCanceled(unit, action);
                }
            });
            
            HandleActionPlanned(unit, action);
            _disposable = Observable.EveryUpdate().Subscribe(_ => Tick(unit, action));
            return UniTask.WaitUntil(() => _disposable == null);
        }

        private void HandleActionPlanned(IUnit unit, UnitAction action) {
            _logger.Log(LoggedFeature.Units, $"Action: {action} planned on unit: {unit.UnitData.Name}");
            _actionListeners.Where(x => x.ActionType == action).ToList().ForEach(x => x.HandleActionPlanned(unit));
        }

        private void HandleActionTick(IUnit unit, UnitAction unitAction) {
            _actionListeners.Where(x => x.ActionType == unitAction).ToList().ForEach(x => x.Tick(unit));
        }

        private void HandleActionConfirmed(IUnit unit, UnitAction action) {
            _logger.Log(LoggedFeature.Units,
                        $"Action: {action} confirmed on unit: {unit.UnitData.Name}.");
            _actionListeners.Where(x => x.ActionType == action).ToList()
                            .ForEach(x => x.HandleActionConfirmed(unit));

            _planCompleteObserver.Dispose();
            _planCompleteObserver = null;
            _disposable.Dispose();
            _disposable = null;
        }

        private void HandleActionCanceled(IUnit unit, UnitAction action) {
            _logger.Log(LoggedFeature.Units, $"Action: {action} canceled on unit: {unit}");
            _actionListeners.Where(x => x.ActionType == action).ToList().ForEach(x => x.HandleActionCanceled(unit));

            _planCompleteObserver.Dispose();
            _planCompleteObserver = null;
            _disposable.Dispose();
            _disposable = null;
        }

        private void Tick(IUnit unit, UnitAction unitAction) {
            HandleActionTick(unit, unitAction);
        }
    }
}