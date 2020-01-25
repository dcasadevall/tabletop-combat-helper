using System;
using System.Collections.Generic;
using Grid;
using MapEditor.MapElement;
using Math;
using UniRx;
using Zenject;

namespace MapEditor {
    public class MapElementSelectionDetector : IInitializable, IDisposable {
        private readonly IMapElementMenuViewController _mapElementMenuViewController;
        private readonly IGridInputManager _gridInputManager;
        private readonly List<IMapEditorTool> _mapEditorTools;

        private IDisposable _observer;

        public MapElementSelectionDetector([Inject(Id = MapEditorInstaller.SECTION_TILE_EDITOR_ID)]
                                           IMapEditorTool sectionTileEditor,
                                           [Inject(Id = MapEditorInstaller.UNIT_TILE_EDITOR_ID)]
                                           IMapEditorTool unitTileEditor,
                                           IMapElementMenuViewController mapElementMenuViewController,
                                           IGridInputManager gridInputManager) {
            _mapElementMenuViewController = mapElementMenuViewController;
            _gridInputManager = gridInputManager;
            
            // TODO: Try to find a way to inject this array even though the implementations are bound with ID.
            _mapEditorTools = new List<IMapEditorTool> {
                sectionTileEditor,
                unitTileEditor
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