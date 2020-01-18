using System;
using System.Collections.Generic;
using Grid;
using Grid.Highlighting;
using InputSystem;
using Logging;
using Math;
using UniRx;
using UnityEngine;
using Zenject;

namespace MapEditor.SectionTiles {
    public class SectionTileEditor : IMapEditorTool {
        private readonly Texture2D _cursorTexture;
        private readonly IGridCellHighlightPool _gridCellHighlightPool;
        private readonly IGridInputManager _gridInputManager;
        private readonly IInputLock _inputLock;
        private readonly EditSectionTileViewController _editSectionTileViewController;
        private IGridCellHighlight _gridCellHighlight;
        private List<IDisposable> _observers = new List<IDisposable>();

        public SectionTileEditor([Inject(Id = MapEditorInstaller.SECTION_TILES_CURSOR)]
                                 Texture2D cursorTexture,
                                 IGridCellHighlightPool gridCellHighlightPool,
                                 IGridInputManager gridInputManager,
                                 IInputLock inputLock,
                                 EditSectionTileViewController editSectionTileViewController) {
            _cursorTexture = cursorTexture;
            _gridCellHighlightPool = gridCellHighlightPool;
            _gridInputManager = gridInputManager;
            _inputLock = inputLock;
            _editSectionTileViewController = editSectionTileViewController;
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
                await _editSectionTileViewController.Show(tileCoords);

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
                _gridCellHighlight = null;
            }

            _gridCellHighlight = _gridCellHighlightPool.Spawn(tileCoords, new Color(1, 0, 0, 0.4f));
        }

        private void SetSectionTileCursor() {
            Cursor.SetCursor(_cursorTexture, Vector2.one * 16, CursorMode.Auto);
        }

        private void SetDefaultCursor() {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}