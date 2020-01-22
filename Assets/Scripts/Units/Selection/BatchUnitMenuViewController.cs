using System;
using System.Linq;
using CommandSystem;
using Grid;
using Grid.Commands;
using Grid.GridUnits;
using Grid.Positioning;
using Logging;
using UI.RadialMenu;
using UniRx;
using UniRx.Async;
using Units.Actions;
using Units.Spawning.Commands;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ILogger = Logging.ILogger;

namespace Units.Selection {
    public class BatchUnitMenuViewController : MonoBehaviour {
        [SerializeField]
        private Button _removeUnitButton;
        
        [SerializeField]
        private Button _rotateUnitButton;

        [SerializeField]
        private Button _cancelSelectionButton;

        private Camera _camera;
        private UnitSelectionHighlighter _unitSelectionHighlighter;
        private ICommandQueue _commandQueue;
        private IUnitActionPlanner _unitActionPlanner;
        private IGridUnitManager _gridUnitManager;
        private IGridUnitInputManager _gridUnitInputManager;
        private IRadialMenu _radialMenu;
        private IGridPositionCalculator _gridPositionCalculator;
        private ILogger _logger;

        // We keep track of the selected units so we can act on them when buttons are pressed.
        private IUnit[] _selectedUnits;
        private IDisposable _observer;

        [Inject]
        public void Construct(Camera camera,
                              UnitSelectionHighlighter unitSelectionHighlighter,
                              ICommandQueue commandQueue,
                              IUnitActionPlanner unitActionPlanner,
                              IGridUnitManager gridUnitManager,
                              IGridUnitInputManager gridUnitInputManager,
                              IGridPositionCalculator gridPositionCalculator,
                              ILogger logger) {
            _camera = camera;
            _commandQueue = commandQueue;
            _unitSelectionHighlighter = unitSelectionHighlighter;
            _unitActionPlanner = unitActionPlanner;
            _gridUnitManager = gridUnitManager;
            _gridUnitInputManager = gridUnitInputManager;
            _gridPositionCalculator = gridPositionCalculator;
            _logger = logger;

            // TODO: Be better
            _radialMenu = GetComponent<IRadialMenu>();
        }

        /// <summary>
        /// Shows the batch selection UI and highlights the selected units.
        /// Returns a task that is completed once the UI has closed or the batch action is complete.
        /// </summary>
        /// <param name="units"></param>
        /// <returns></returns>
        public async UniTask ShowAndWaitForAction(IUnit[] units) {
            // Show the menu
            var unitCoords = _gridUnitManager.GetUnitCoords(units[0]);
            if (unitCoords == null) {
                var msg = $"Unit not in tile: {units[0]}";
                _logger.LogError(LoggedFeature.Units, msg);
                throw new Exception(msg);
            }

            _selectedUnits = units;
            var worldPosition = _gridPositionCalculator.GetTileCenterWorldPosition(unitCoords.Value);
            _radialMenu.Show(_camera.WorldToScreenPoint(worldPosition));

            // Add listeners
            _removeUnitButton.onClick.AddListener(HandleRemoveUnitPressed);
            _rotateUnitButton.onClick.AddListener(HandleRotateUnitPressed);
            _cancelSelectionButton.onClick.AddListener(HandleCancelSelectionPressed);

            // Highlights
            _unitSelectionHighlighter.HighlightUnits(units);

            // Mouse Up / Down streams
            var mouseUpStream = Observable.EveryUpdate()
                                          .Where(_ => Input.GetMouseButtonUp(0));
            var mouseDownStream = Observable.EveryUpdate()
                                            .Where(_ => Input.GetMouseButtonDown(0))
                                            .Where(_ => _gridUnitInputManager.UnitsAtMousePosition.Length > 0)
                                            .Where(_ => units.Intersect(_gridUnitInputManager.UnitsAtMousePosition).Any())
                                            .First();

            _observer = mouseDownStream.Select(_ => mouseUpStream).Subscribe(_ => {
                HandleUnitMouseDown(units);
            });

            await UniTask.WaitUntil(() => _selectedUnits == null);
        }

        private async void HandleUnitMouseDown(IUnit[] units) {
            Hide();
            await _unitActionPlanner.PlanBatchedAction(units, UnitAction.DragAndDrop);
            _unitSelectionHighlighter.ClearHighlights();
            _selectedUnits = null;
        }

        public void HandleRotateUnitPressed() {
            foreach (var selectedUnit in _selectedUnits) {
                _commandQueue.Enqueue<RotateUnitCommand, RotateUnitData>(new RotateUnitData(selectedUnit.UnitId, 90),
                                                                         CommandSource.Game);
            }
        }

        public void HandleRemoveUnitPressed() {
            foreach (var selectedUnit in _selectedUnits) {
                _commandQueue.Enqueue<DespawnUnitCommand, DespawnUnitData>(new DespawnUnitData(selectedUnit.UnitId),
                                                                           CommandSource.Game);
            }
            
            Hide();
            _unitSelectionHighlighter.ClearHighlights();
            _selectedUnits = null;
        }

        public void HandleCancelSelectionPressed() {
            Hide();
            _unitSelectionHighlighter.ClearHighlights();
            _selectedUnits = null;
        }

        private void Hide() {
            // Listeners
            _removeUnitButton.onClick.RemoveListener(HandleRemoveUnitPressed);
            _rotateUnitButton.onClick.RemoveListener(HandleRotateUnitPressed);
            _cancelSelectionButton.onClick.RemoveListener(HandleCancelSelectionPressed);

            // Hide Menu
            _radialMenu.Hide();

            // Dispose observer
            _observer?.Dispose();
            _observer = null;
        }
    }
}