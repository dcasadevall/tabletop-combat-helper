using System;
using System.Threading;
using System.Threading.Tasks;
using Grid;
using Grid.Highlighting;
using InputSystem;
using Logging;
using Math;
using UniRx;
using UniRx.Async;
using Units.Spawning.UI;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;
using ILogger = Logging.ILogger;

namespace Units.Selection {
    /// <summary>
    /// This MonoBehaviour controls collapsing / expanding the unit toolbar, which can be used to spawn / batch select
    /// units.
    /// </summary>
    public class UnitToolbarViewController : MonoBehaviour, IInitializable, ITickable, IDisposable {
        [SerializeField]
        private Button _addUnitsButton;

        [SerializeField]
        private Button _batchSelectButton;

        [SerializeField]
        private Button _cancelButton;

        [SerializeField]
        private GameObject _buttonGroup;

        [SerializeField]
        private GameObject _cancelGroup;

        private BatchUnitSelectionDetector _batchUnitSelectionDetector;
        private BatchUnitMenuViewController _batchUnitMenuViewController;
        private IGridCellHighlightPool _gridCellHighlightPool;
        private IGridInputManager _gridInputManager;
        private IUnitSpawnViewController _unitSpawnViewController;
        private IInputLock _inputLock;
        private ILogger _logger;

        private IDisposable _lockToken;
        private IDisposable _selectionObserver;
        private IGridCellHighlight _gridCellHighlight;
        private CancellationTokenSource _cancellationTokenSource;

        [Inject]
        internal void Construct(BatchUnitSelectionDetector batchUnitSelectionDetector,
                                BatchUnitMenuViewController batchUnitMenuViewController,
                                IGridCellHighlightPool gridCellHighlightPool,
                                IGridInputManager gridInputManager,
                                IUnitSpawnViewController unitSpawnViewController,
                                IInputLock inputLock,
                                ILogger logger) {
            _batchUnitSelectionDetector = batchUnitSelectionDetector;
            _batchUnitMenuViewController = batchUnitMenuViewController;
            _gridCellHighlightPool = gridCellHighlightPool;
            _gridInputManager = gridInputManager;
            _unitSpawnViewController = unitSpawnViewController;
            _inputLock = inputLock;
            _logger = logger;

            Preconditions.CheckNotNull(_addUnitsButton, _batchSelectButton, _cancelButton, _buttonGroup, _cancelGroup);
        }

        public void Initialize() {
            _inputLock.InputLockAcquired += HandleInputLockAcquired;
            _inputLock.InputLockReleased += HandleInputLockReleased;

            _addUnitsButton.onClick.AddListener(HandleAddUnitsPressed);
            _batchSelectButton.onClick.AddListener(HandleBatchSelectPressed);
            _cancelButton.onClick.AddListener(HandleCancelButtonPressed);
        }

        // TODO: This is a pretty janky way of handling input.
        // Maybe create a separate class to handle this?
        // Or disable these shortcuts altogether...
        public void Tick() {
            if (!gameObject.activeInHierarchy) {
                return;
            }

            if (Input.GetKeyUp(KeyCode.U)) {
                HandleAddUnitsPressed();
            }
        }

        public void Dispose() {
            _inputLock.InputLockAcquired -= HandleInputLockAcquired;
            _inputLock.InputLockReleased -= HandleInputLockReleased;

            _addUnitsButton.onClick.RemoveListener(HandleAddUnitsPressed);
            _batchSelectButton.onClick.RemoveListener(HandleBatchSelectPressed);
            _cancelButton.onClick.RemoveListener(HandleCancelButtonPressed);
        }

        private void Show() {
            gameObject.SetActive(true);
        }

        private void Hide() {
            _selectionObserver?.Dispose();
            _selectionObserver = null;

            gameObject.SetActive(false);
        }

        private void ShowToolbar() {
            _buttonGroup.SetActive(true);
            _cancelGroup.SetActive(false);
        }

        private void ShowCancelButton() {
            _buttonGroup.SetActive(false);
            _cancelGroup.SetActive(true);
        }

        private void HandleInputLockAcquired() {
            // Note that this works only because we parent our objects under the scene context.
            // TODO: Instead of injecting unitSelection with MapSectionScene, all UI should be "encounter" UI
            // Otherwise, Show / Hide will show ui from hidden map sections unless we parent them under the scene ctx.
            Hide();
        }

        private void HandleInputLockReleased() {
            Show();
        }

        private async void HandleAddUnitsPressed() {
            using (_lockToken = _inputLock.Lock()) {
                await Task.Run(() => WaitForClickAndShowSpawnViewController(), _cancellationTokenSource.Token);
            }
        }

        private async void WaitForClickAndShowSpawnViewController() {
            Show();
            ShowCancelButton();

            // TODO: Create a "Highlight Mouse On Tile" reusable method.
            // Note that we don't use 'using' because we need the variable tileCoords out of that scope.
            IDisposable observer = _gridInputManager.MouseEnteredTile.Subscribe(HandleMouseEnteredTile);
            IntVector2 tileCoords = await Task.Run(() => _gridInputManager.LeftMouseButtonOnTile.ToTask(), _cancellationTokenSource.Token);
            observer.Dispose();
            
            await _unitSpawnViewController.Show(tileCoords);
            
            HideHighlight();
            ShowToolbar();
        }

        private async void HandleTileClicked(IntVector2 tileCoords) {
        }

        private void HandleMouseEnteredTile(IntVector2 tileCoords) {
            HideHighlight();
            _gridCellHighlight = _gridCellHighlightPool.Spawn(tileCoords, new Color(1, 0, 0, 0.4f));
        }

        private void HideHighlight() {
            if (_gridCellHighlight != null) {
                _gridCellHighlightPool.Despawn(_gridCellHighlight);
                _gridCellHighlight = null;
            }
        }

        private async void HandleBatchSelectPressed() {
            using (_lockToken = _inputLock.Lock()) {
                Show(); // See essay above.
                ShowCancelButton();

                _cancellationTokenSource = new CancellationTokenSource();
                using (_batchUnitSelectionDetector
                       .GetSelectedUnitsObservable().Subscribe(HandleUnitsSelected)) {
                    await UniTask.WaitUntil(() => _cancellationTokenSource.Token.IsCancellationRequested);
                }

                ShowToolbar();
            }
        }

        private async void HandleUnitsSelected(IUnit[] units) {
            _logger.Log(LoggedFeature.Units, "Selected {0} Units", units.ToString());
            Hide();
            await _batchUnitMenuViewController.ShowAndWaitForAction(units);
            Show();
            _cancellationTokenSource.Cancel();
        }

        private void HandleCancelButtonPressed() {
            _cancellationTokenSource.Cancel();
        }
    }
}