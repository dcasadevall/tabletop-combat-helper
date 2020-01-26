using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grid;
using Grid.Highlighting;
using InputSystem;
using MapEditor.MapElement;
using Math;
using UniRx;
using UniRx.Async;
using UnityEngine;

namespace MapEditor.SingleTileEditor {
    /// <summary>
    /// <see cref="IMapEditorTool"/> responsible for detecting tile clicks and showing a cursor / view controller /
    /// grid highlight with cursor.
    /// This can be reused for different <see cref="IMapEditorTool"/>s that share this input / visualization.
    /// </summary>
    public class SingleTileMapEditorTool : IMapEditorTool {
        private readonly ISingleTileMapEditorToolDelegate _delegate;
        private readonly IGridCellHighlightPool _gridCellHighlightPool;
        private readonly IGridInputManager _gridInputManager;
        private readonly List<IDisposable> _observers = new List<IDisposable>();

        private CancellationTokenSource _cancellationTokenSource;
        private IGridCellHighlight _gridCellHighlight;

        public SingleTileMapEditorTool(ISingleTileMapEditorToolDelegate @delegate,
                                       IGridCellHighlightPool gridCellHighlightPool,
                                       IGridInputManager gridInputManager) {
            _delegate = @delegate;
            _gridCellHighlightPool = gridCellHighlightPool;
            _gridInputManager = gridInputManager;
        }

        public IMapElement MapElementAtTileCoords(IntVector2 tileCoords) {
            return _delegate.MapElementAtTileCoords(tileCoords);
        }

        public void StartEditing() {
            _gridInputManager.LeftMouseButtonOnTile
                             .Where(_ => _cancellationTokenSource == null)
                             .Subscribe(HandleTileClicked)
                             .AddTo(_observers);

            _gridInputManager.MouseEnteredTile
                             .Where(_ => _cancellationTokenSource == null)
                             .Subscribe(HandleMouseOnTile)
                             .AddTo(_observers);

            SetSectionTileCursor();
        }

        public void StopEditing() {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = null;
            
            if (_gridCellHighlight != null) {
                _gridCellHighlightPool.Despawn(_gridCellHighlight);
                _gridCellHighlight = null;
            }

            _observers.ForEach(x => x.Dispose());
            SetDefaultCursor();
        }

        private async void HandleTileClicked(IntVector2 tileCoords) {
            // Trigger highlight for mobile since mouse over does not happen there.
            HandleMouseOnTile(tileCoords);
            SetDefaultCursor();

            // Allow cancellation when "StopEditing" is called.
            _cancellationTokenSource = new CancellationTokenSource();
            // The task is ran "as uni task" to avoid opening a new thread.
            // We are showing a view controller which will require MonoBehaviours (needs main thread).
            await _delegate.Show(tileCoords, _cancellationTokenSource.Token).SuppressCancellationThrow();
            _cancellationTokenSource = null;

            // Update highlight to match new mouse position
            _gridCellHighlightPool.Despawn(_gridCellHighlight);
            _gridCellHighlight = null;
            if (_gridInputManager.TileAtMousePosition != null) {
                HandleMouseOnTile(_gridInputManager.TileAtMousePosition.Value);
            }

            SetSectionTileCursor();
        }

        private void HandleMouseOnTile(IntVector2 tileCoords) {
            if (_gridCellHighlight != null) {
                _gridCellHighlightPool.Despawn(_gridCellHighlight);
            }

            _gridCellHighlight = _gridCellHighlightPool.Spawn(tileCoords, new Color(1, 0, 0, 0.4f));
        }

        private void SetSectionTileCursor() {
            Cursor.SetCursor(_delegate.CursorTexture, Vector2.one * 16, CursorMode.Auto);
        }

        private void SetDefaultCursor() {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}