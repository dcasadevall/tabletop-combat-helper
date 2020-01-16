using System;
using System.Linq;
using CameraSystem;
using CommandSystem;
using Grid;
using Grid.Commands;
using Grid.Positioning;
using Logging;
using Math;
using UniRx;
using Units.Actions;
using Units.Spawning;
using UnityEngine;
using Utils;
using ILogger = Logging.ILogger;

namespace Units.Movement.ActionHandlers {
    /// <summary>
    /// Action handler used to drag a unit around the grid, without movement restrictions.
    /// Action is confirmed when drag stops.
    /// Action is never canceled.
    /// </summary>
    public class UnitDragAndDropHandler : ISingleUnitActionHandler, IBatchedUnitActionHandler {
        private readonly IGridInputManager _gridInputManager;
        private readonly IGridUnitManager _gridUnitManager;
        private readonly IGridPositionCalculator _gridPositionCalculator;
        private readonly ICommandQueue _commandQueue;
        private readonly IUnitTransformRegistry _unitTransformRegistry;
        private readonly ILogger _logger;

        private IntVector2? _startCoordinate;
        private IDisposable _disposable;

        public UnitAction ActionType {
            get {
                return UnitAction.DragAndDrop;
            }
        }

        public IObservable<UniRx.Unit> ConfirmActionObservable {
            get {
                return Observable.EveryUpdate()
                                 .Where(_ => !Input.GetMouseButton(0))
                                 .Select(_ => UniRx.Unit.Default);
            }
        }

        public IObservable<UniRx.Unit> CancelActionObservable {
            get {
                return Observable.Never<UniRx.Unit>();
            }
        }

        public UnitDragAndDropHandler(IGridInputManager gridInputManager,
                                      IGridUnitManager gridUnitManager,
                                      IGridPositionCalculator gridPositionCalculator,
                                      ICommandQueue commandQueue,
                                      IUnitTransformRegistry unitTransformRegistry,
                                      ILogger logger) {
            _gridInputManager = gridInputManager;
            _gridUnitManager = gridUnitManager;
            _gridPositionCalculator = gridPositionCalculator;
            _commandQueue = commandQueue;
            _unitTransformRegistry = unitTransformRegistry;
            _logger = logger;
        }

        public void HandleActionPlanned(IUnit unit) {
            _disposable = _gridInputManager.MouseEnteredTile.Subscribe(coords => {
                PreviewUnitMovement(unit, coords);
            });
        }

        public void HandleActionConfirmed(IUnit unit) {
            // These can be checked (throw exception if null) because our stream guarantees that they are not.
            IntVector2 unitCoords = _gridUnitManager.GetUnitCoords(unit).GetValueChecked();
            IntVector2 mouseCoords = _gridInputManager.TileAtMousePosition.GetValueChecked();
            CommitUnitMovement(unit, mouseCoords - unitCoords);

            _disposable?.Dispose();
            _disposable = null;
        }

        public void HandleActionCanceled(IUnit unit) {
            CancelUnitMovement(unit);

            _disposable?.Dispose();
            _disposable = null;
        }

        public void HandleActionPlanned(IUnit[] units) {
            if (_gridInputManager.TileAtMousePosition == null) {
                _logger.LogError(LoggedFeature.Units, "Drag and drop action planned on units when not inside grid.");
                return;
            }

            _startCoordinate = _gridInputManager.TileAtMousePosition.Value;
            var unitStartingCoordinates = units.ToDictionary(unit => unit,
                                                             unit => _gridUnitManager.GetUnitCoords(unit)
                                                                                     .GetValueChecked());
            _disposable = _gridInputManager.MouseEnteredTile.Subscribe(coords => {
                IntVector2 mouseCoords = _gridInputManager.TileAtMousePosition.GetValueChecked();
                
                foreach (var unit in units) {
                    IntVector2 moveDistance = mouseCoords - _startCoordinate.GetValueChecked();
                    IntVector2 unitStart = unitStartingCoordinates[unit];
                    PreviewUnitMovement(unit, unitStart + moveDistance);
                }
            });
        }

        public void HandleActionConfirmed(IUnit[] units) {
            // These can be checked because our stream guarantees that they are not.
            IntVector2 mouseCoords = _gridInputManager.TileAtMousePosition.GetValueChecked();
            foreach (var unit in units) {
                IntVector2 moveDistance = mouseCoords - _startCoordinate.GetValueChecked();
                CommitUnitMovement(unit, moveDistance);
            }

            _disposable?.Dispose();
            _disposable = null;
        }

        public void HandleActionCanceled(IUnit[] units) {
            foreach (var unit in units) {
                CancelUnitMovement(unit);
            }

            _disposable?.Dispose();
            _disposable = null;
        }

        private void PreviewUnitMovement(IUnit unit, IntVector2 previewCoords) {
            // Move the unit in space, but do not actually execute a move command (since we are previewing)
            ITransformableUnit transformableUnit = _unitTransformRegistry.GetTransformableUnit(unit.UnitId);
            Vector3 worldPosition = _gridPositionCalculator.GetTileCenterWorldPosition(previewCoords);
            transformableUnit.Transform.position =
                new Vector3(worldPosition.x, worldPosition.y, transformableUnit.Transform.position.z);
        }

        private void CommitUnitMovement(IUnit unit, IntVector2 moveDistance) {
            if (moveDistance == IntVector2.Zero) {
                _logger.Log(LoggedFeature.Units, "MoveDistance is 0. Will not commit command.");
                return;
            }

            _commandQueue.Enqueue<MoveUnitCommand, MoveUnitData>(new MoveUnitData(unit.UnitId,
                                                                                  moveDistance),
                                                                 CommandSource.Game);
        }

        private void CancelUnitMovement(IUnit unit) {
            // Stream guarantees non null coords, so we can use checked value.
            IntVector2 unitCoords = _gridUnitManager.GetUnitCoords(unit).GetValueChecked();
            PreviewUnitMovement(unit, unitCoords);
        }
    }
}