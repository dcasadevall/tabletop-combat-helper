using System;
using System.Linq;
using Grid;
using Grid.Positioning;
using Logging;
using UI.RadialMenu;
using UniRx;
using UniRx.Async;
using Units.Actions;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ILogger = Logging.ILogger;

namespace Units.Selection {
    public class BatchUnitMenuViewController : MonoBehaviour {
        [SerializeField]
        private Button _removeUnitButton;

        [SerializeField]
        private Button _cancelSelectionButton;

        private Camera _camera;
        private UnitSelectionHighlighter _unitSelectionHighlighter;
        private IUnitActionPlanner _unitActionPlanner;
        private IGridUnitManager _gridUnitManager;
        private IGridInputManager _gridInputManager;
        private IRadialMenu _radialMenu;
        private IGridPositionCalculator _gridPositionCalculator;
        private ILogger _logger;

        // We need this boolean flag because we may still hide the menu while awaiting on drag / drop action.
        private bool _isActive;
        private IDisposable _observer;

        [Inject]
        public void Construct(Camera camera,
                              UnitSelectionHighlighter unitSelectionHighlighter,
                              IUnitActionPlanner unitActionPlanner,
                              IGridUnitManager gridUnitManager,
                              IGridInputManager gridInputManager,
                              IGridPositionCalculator gridPositionCalculator,
                              ILogger logger) {
            _camera = camera;
            _unitSelectionHighlighter = unitSelectionHighlighter;
            _unitActionPlanner = unitActionPlanner;
            _gridUnitManager = gridUnitManager;
            _gridInputManager = gridInputManager;
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

            _isActive = true;
            var worldPosition = _gridPositionCalculator.GetTileCenterWorldPosition(unitCoords.Value);
            _radialMenu.Show(_camera.WorldToScreenPoint(worldPosition));
            
            // Add listeners
            _removeUnitButton.onClick.AddListener(HandleRemoveUnitPressed);
            _cancelSelectionButton.onClick.AddListener(HandleCancelSelectionPressed);

            // Highlights
            _unitSelectionHighlighter.HighlightUnits(units);

            // Mouse Up / Down streams
            var mouseUpStream = Observable.EveryUpdate()
                                          .Where(_ => Input.GetMouseButtonUp(0));
            var mouseDownStream = Observable.EveryUpdate()
                                            .Where(_ => Input.GetMouseButtonDown(0))
                                            .Where(_ => _gridInputManager.UnitsAtMousePosition.Length > 0)
                                            .Where(_ => units.Intersect(_gridInputManager.UnitsAtMousePosition).Any())
                                            .First();

            _observer = mouseDownStream.Select(_ => mouseUpStream).Subscribe(_ => {
                HandleUnitMouseDown(units);
            });

            await UniTask.WaitUntil(() => !_isActive);
        }

        private async void HandleUnitMouseDown(IUnit[] units) {
            Hide();
            await _unitActionPlanner.PlanBatchedAction(units, UnitAction.DragAndDrop);
            _unitSelectionHighlighter.ClearHighlights();
            _isActive = false;
        }

        public void HandleRemoveUnitPressed() {
            Hide();
            _unitSelectionHighlighter.ClearHighlights();
            _isActive = false;
        }

        public void HandleCancelSelectionPressed() {
            Hide();
            _unitSelectionHighlighter.ClearHighlights();
            _isActive = false;
        }

        private void Hide() {
            // Listeners
            _removeUnitButton.onClick.RemoveListener(HandleRemoveUnitPressed);
            _cancelSelectionButton.onClick.RemoveListener(HandleCancelSelectionPressed);

            // Hide Menu
            _radialMenu.Hide();

            // Dispose observer
            _observer?.Dispose();
            _observer = null;
        }
    }
}