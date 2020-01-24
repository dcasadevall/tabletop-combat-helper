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
        private readonly IGridCellHighlighter _gridCellHighlighter;
        private readonly IGridInputManager _gridInputManager;
        private readonly IInputLock _inputLock;
        
        private IDisposable _highlightObserver;
        private IDisposable _tileClickObserver;

        public SingleTileMapEditorTool(ISingleTileMapEditorToolDelegate @delegate,
                                       IGridCellHighlighter gridCellHighlighter,
                                       IGridInputManager gridInputManager,
                                       IInputLock inputLock) {
            _delegate = @delegate;
            _gridCellHighlighter = gridCellHighlighter;
            _gridInputManager = gridInputManager;
            _inputLock = inputLock;
        }

        public IMapElement MapElementAtTileCoords(IntVector2 tileCoords) {
            return _delegate.MapElementAtTileCoords(tileCoords);
        }

        public void StartEditing() {
            _tileClickObserver = _gridInputManager.LeftMouseButtonOnTile
                                                  .Where(_ => !_inputLock.IsLocked)
                                                  .Subscribe(HandleTileClicked);

            _highlightObserver = _gridCellHighlighter.HighlightCellOnMouseOver(stayHighlighted: true);

            SetSectionTileCursor();
        }

        public void StopEditing() {
            _gridCellHighlighter.ClearHighlight();
            _tileClickObserver?.Dispose();
            _tileClickObserver = null;
            _highlightObserver?.Dispose();
            _highlightObserver = null;
            SetDefaultCursor();
        }

        private async void HandleTileClicked(IntVector2 tileCoords) {
            // Trigger highlight for mobile since mouse over does not happen there.
            _gridCellHighlighter.SetHighlight(tileCoords);

            using (_inputLock.Lock()) {
                SetDefaultCursor();
                
                // Stop highlighting on mouse over.
                _highlightObserver?.Dispose();
                _highlightObserver = null;
                // Update highlight to match new mouse position.
                _gridCellHighlighter.ClearHighlight();
                if (_gridInputManager.TileAtMousePosition != null) {
                    _gridCellHighlighter.SetHighlight(_gridInputManager.TileAtMousePosition.Value);
                }

                await _delegate.Show(tileCoords);
                
                _gridCellHighlighter.ClearHighlight();
                _highlightObserver = _gridCellHighlighter.HighlightCellOnMouseOver(stayHighlighted: true);
                SetSectionTileCursor();
            }
        }

        private void SetSectionTileCursor() {
            Cursor.SetCursor(_delegate.CursorTexture, Vector2.one * 16, CursorMode.Auto);
        }

        private void SetDefaultCursor() {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}