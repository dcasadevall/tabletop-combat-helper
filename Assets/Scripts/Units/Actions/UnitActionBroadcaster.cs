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
        private readonly IGridInputManager _gridInputManager;
        private readonly ILogger _logger;
        private IDisposable _disposable;

        UnitActionBroadcaster(List<IUnitActionListener> actionListeners, IGridInputManager gridInputManager,
                              ILogger logger) {
            _actionListeners = actionListeners;
            _gridInputManager = gridInputManager;
            _logger = logger;
        }

        public UniTask PlanAction(IUnit unit, UnitAction action) {
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
        
        private void HandleActionConfirmed(IUnit unit, UnitAction action, IntVector2 tileCoords) {
            _logger.Log(LoggedFeature.Units,
                        $"Action: {action} confirmed on unit: {unit.UnitData.Name}. Coords: {tileCoords}");
            _actionListeners.Where(x => x.ActionType == action).ToList()
                            .ForEach(x => x.HandleActionConfirmed(unit, tileCoords));
            
            _disposable.Dispose();
            _disposable = null;
        }

        private void HandleActionCanceled(IUnit unit, UnitAction action) {
            _logger.Log(LoggedFeature.Units, $"Action: ${action} canceled on unit: ${unit}");
            _actionListeners.Where(x => x.ActionType == action).ToList().ForEach(x => x.HandleActionCanceled(unit));
            
            _disposable.Dispose();
            _disposable = null;
        }

        private void Tick(IUnit unit, UnitAction unitAction) {
            HandleActionTick(unit, unitAction);
            
            if (Input.GetMouseButtonDown(1)) {
                HandleActionCanceled(unit, unitAction);
                return;
            }

            if (Input.GetMouseButtonDown(0)) {
                IntVector2? currentTile = _gridInputManager.GetTileAtMousePosition();
                if (currentTile != null) {
                    HandleActionConfirmed(unit, unitAction, currentTile.Value);
                }
            }
        }
    }
}