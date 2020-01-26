using System.Threading;
using Map.MapData;
using MapEditor.MapElement;
using MapEditor.SingleTileEditor;
using Math;
using UniRx.Async;
using UnityEngine;
using Zenject;

namespace MapEditor.SectionTiles {
    public class SectionTileMapEditorTool : ISingleTileMapEditorToolDelegate {
        private readonly EditSectionTileViewController _editSectionTileViewController;
        private readonly IMutableMapSectionData _mapSectionData;
        private readonly Texture2D _cursorTexture;
        public Texture2D CursorTexture {
            get {
                return _cursorTexture;
            }
        }

        public SectionTileMapEditorTool([Inject(Id = MapEditorInstaller.SECTION_TILES_CURSOR)]
                                        Texture2D cursorTexture,
                                        EditSectionTileViewController editSectionTileViewController,
                                        IMutableMapSectionData mapSectionData) {
            _cursorTexture = cursorTexture;
            _editSectionTileViewController = editSectionTileViewController;
            _mapSectionData = mapSectionData;
        }

        public UniTask Show(IntVector2 tileCoords, CancellationToken cancellationToken) {
            return _editSectionTileViewController.Show(tileCoords, cancellationToken);
        }

        public IMapElement MapElementAtTileCoords(IntVector2 tileCoords) {
            if (!_mapSectionData.TileMetadataMap.ContainsKey(tileCoords)) {
                return null;
            }
            
            return new SectionTileMapElement(_mapSectionData, tileCoords);
        }
    }
}