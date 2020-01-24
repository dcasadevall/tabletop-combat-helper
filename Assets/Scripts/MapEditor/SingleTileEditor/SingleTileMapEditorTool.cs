using System;
using System.Collections.Generic;
using Grid;
using Grid.Highlighting;
using InputSystem;
using MapEditor.MapElement;
using Math;
using UniRx;
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
        private readonly IInputLock _inputLock;
        private IGridCellHighlight _gridCellHighlight;
        private List<IDisposable> _observers = new List<IDisposable>();

        public SingleTileMapEditorTool(ISingleTileMapEditorToolDelegate @delegate,
                                       IGridCellHighlightPool gridCellHighlightPool,
                                       IGridInputManager gridInputManager,
                                       IInputLock inputLock) {
            _delegate = @delegate;
            _gridCellHighlightPool = gridCellHighlightPool;
            _gridInputManager = gridInputManager;
            _inputLock = inputLock;
        }

        public IMapElement MapElementAtTileCoords(IntVector2 tileCoords) {
            return _delegate.MapElementAtTileCoords(tileCoords);
        }

        public void StartEditing() {
            _gridInputManager.LeftMouseButtonOnTile
                             .Where(_ => !_inputLock.IsLocked)
                             .Subscribe(HandleTileClicked).AddTo(_observers);

            _gridInputManager.MouseEnteredTile
                             .Where(_ => !_inputLock.IsLocked)
                             .Subscribe(HandleMouseOnTile)
                             .AddTo(_observers);

            SetSectionTileCursor();
        }

        public void StopEditing() {
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

            using (_inputLock.Lock()) {
                SetDefaultCursor();
                await _delegate.Show(tileCoords);

                // Update highlight to match new mouse position
                _gridCellHighlightPool.Despawn(_gridCellHighlight);
                _gridCellHighlight = null;
                if (_gridInputManager.TileAtMousePosition != null) {
                    HandleMouseOnTile(_gridInputManager.TileAtMousePosition.Value);
                }

                SetSectionTileCursor();
            }
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