using System;
using System.Collections.Generic;
using System.Linq;
using Grid;
using InputSystem;
using MapEditor.MapElement;
using Math;
using UniRx;
using UnityEngine;
using Zenject;

namespace MapEditor {
    public class MapElementSelectionDetector : IInitializable, IDisposable {
        private readonly IMapElementMenuViewController _mapElementMenuViewController;
        private readonly IGridInputManager _gridInputManager;
        private readonly IInputLock _inputLock;
        private readonly List<IMapEditorTool> _mapEditorTools;

        private IDisposable _observer;

        public MapElementSelectionDetector([Inject(Id = MapEditorInstaller.SECTION_TILE_EDITOR_ID)]
                                           IMapEditorTool sectionTileEditor,
                                           [Inject(Id = MapEditorInstaller.UNIT_TILE_EDITOR_ID)]
                                           IMapEditorTool unitTileEditor,
                                           IMapElementMenuViewController mapElementMenuViewController,
                                           IGridInputManager gridInputManager,
                                           IInputLock inputLock) {
            _mapElementMenuViewController = mapElementMenuViewController;
            _gridInputManager = gridInputManager;
            _inputLock = inputLock;

            // TODO: Try to find a way to inject this array even though the implementations are bound with ID.
            _mapEditorTools = new List<IMapEditorTool> {
                sectionTileEditor,
                unitTileEditor
            };
        }

        public void Initialize() {
            _observer = _gridInputManager.LeftMouseButtonOnTile
                                         .Where(_ => !_inputLock.IsLocked)
                                         .Subscribe(HandleTileClicked);
        }

        public void Dispose() {
            _observer?.Dispose();
            _observer = null;
        }

        private async void HandleTileClicked(IntVector2 tileCoords) {
            var mapElement = _mapEditorTools.Select(editor => editor.MapElementAtTileCoords(tileCoords))
                                            .FirstOrDefault(x => x != null);
            if (mapElement == null) {
                return;
            }

            using (_inputLock.Lock()) {
                await _mapElementMenuViewController.Show(tileCoords, mapElement);
            }
        }
    }
}