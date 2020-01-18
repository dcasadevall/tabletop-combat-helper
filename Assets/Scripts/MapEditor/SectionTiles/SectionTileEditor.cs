using System;
using Grid;
using InputSystem;
using Logging;
using Math;
using UniRx;

namespace MapEditor.SectionTiles {
    public class SectionTileEditor : IMapEditorTool {
        private readonly IGridInputManager _gridInputManager;
        private readonly IInputLock _inputLock;
        private readonly EditSectionTileViewController _editSectionTileViewController;
        private IDisposable _observer;

        public SectionTileEditor(IGridInputManager gridInputManager,
                                 IInputLock inputLock,
                                 EditSectionTileViewController editSectionTileViewController) {
            _gridInputManager = gridInputManager;
            _inputLock = inputLock;
            _editSectionTileViewController = editSectionTileViewController;
        }

        public void StartEditing() {
            _observer = _gridInputManager.LeftMouseButtonOnTile
                                         .Where(_ => !_inputLock.IsLocked)
                                         .Subscribe(HandleTileClicked);
        }

        public void StopEditing() {
            _observer?.Dispose();
            _observer = null;
        }

        private async void HandleTileClicked(IntVector2 tileCoords) {
            using (_inputLock.Lock()) {
                await _editSectionTileViewController.Show(tileCoords);
            }
        }
    }
}