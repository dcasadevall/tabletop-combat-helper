using System;
using System.Collections.Generic;
using Grid;
using Math;
using UniRx;
using Zenject;

namespace MapEditor.MapElement {
    public class MapElementSelectionDetector : IInitializable, IDisposable {
        private readonly IMapEditorTool _sectionTileEditor;
        private readonly IMapElementMenuViewController _mapElementMenuViewController;
        private readonly IGridInputManager _gridInputManager;
        private readonly List<IMapEditorTool> _mapEditorTools;

        private IDisposable _observer;

        public MapElementSelectionDetector([Inject(Id = MapEditorInstaller.SECTION_TILE_EDITOR_ID)]
                                           IMapEditorTool sectionTileEditor,
                                           IMapElementMenuViewController mapElementMenuViewController,
                                           IGridInputManager gridInputManager) {
            _sectionTileEditor = sectionTileEditor;
            _mapElementMenuViewController = mapElementMenuViewController;
            _gridInputManager = gridInputManager;
            
            // TODO: Try to find a way to inject this array even though the implementations are bound with ID.
            _mapEditorTools = new List<IMapEditorTool> {
                sectionTileEditor
            };
        }

        public void Initialize() {
            _observer = _gridInputManager.LeftMouseButtonOnTile.Subscribe(HandleTileClicked);
        }

        public void Dispose() {
            _observer?.Dispose();
            _observer = null;
        }

        private void HandleTileClicked(IntVector2 tileCoords) {
            foreach (var mapEditorTool in _mapEditorTools) {
                IMapElement mapElement = mapEditorTool.MapElementAtTileCoords(tileCoords);
                if (mapElement != null) {
                    _mapElementMenuViewController.Show(tileCoords, mapElement);
                    break;
                }
            }
        }
    }
}