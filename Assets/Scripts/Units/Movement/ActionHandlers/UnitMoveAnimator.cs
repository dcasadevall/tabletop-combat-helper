using System;
using CommandSystem;
using Grid;
using Grid.Commands;
using Logging;
using Math;
using UniRx;
using Units.Actions;

namespace Units.Movement.ActionHandlers {
    /// <summary>
    /// Action handler which shows the animation of a unit moving through a specific path.
    /// Action is confirmed once the unit reaches its destination.
    /// Action is never canceled.
    /// </summary>
    public class UnitMoveAnimator : ISingleUnitActionHandler {
        private readonly IGridUnitManager _gridUnitManager;
        private readonly IGridInputManager _gridInputManager;
        private readonly ICommandQueue _commandQueue;
        private readonly ILogger _logger;

        public UnitAction ActionType {
            get {
                return UnitAction.AnimateMovement;
            }
        }

        public IObservable<UniRx.Unit> ConfirmActionObservable {
            get {
                return Observable.Return(UniRx.Unit.Default);
            }
        }

        public IObservable<UniRx.Unit> CancelActionObservable {
            get {
                return Observable.Never<UniRx.Unit>();
            }
        }

        public UnitMoveAnimator(IGridUnitManager gridUnitManager, IGridInputManager gridInputManager,
                                ICommandQueue commandQueue, ILogger logger) {
            _gridUnitManager = gridUnitManager;
            _gridInputManager = gridInputManager;
            _commandQueue = commandQueue;
            _logger = logger;
        }

        public void HandleActionPlanned(IUnit unit) {
            if (!_gridInputManager.TileAtMousePosition.HasValue) {
                return;
            }
            
            IntVector2? unitCoords = _gridUnitManager.GetUnitCoords(unit);
            if (unitCoords == null) {
                _logger.LogError(LoggedFeature.Units, "UnitCoords not found for unit: {0}", unit);
                return;
            }

            IntVector2 moveDistance = _gridInputManager.TileAtMousePosition.Value - unitCoords.Value;
            _commandQueue.Enqueue<MoveUnitCommand, MoveUnitData>(new MoveUnitData(unit.UnitId, moveDistance),
                                                                 CommandSource.Game);
        }

        public void HandleActionConfirmed(IUnit unit) { }
        public void HandleActionCanceled(IUnit unit) { }
    }
}