using System;
using Map.MapData;
using MapEditor.MapElement;
using Math;

namespace MapEditor.SectionTiles {
    public class SectionTileMapElement : IMapElement {
        private readonly IMutableMapSectionData _mutableMapSectionData;

        private uint _sectionIndex;
        private IntVector2 _tileCoords;

        public SectionTileMapElement(IMutableMapSectionData mutableMapSectionData, IntVector2 tileCoords) {
            _mutableMapSectionData = mutableMapSectionData;
            _tileCoords = tileCoords;
        }
        
        public void HandleDrag(IntVector2 tileCoords) {
            if (_tileCoords == tileCoords) {
                return;
            }
            
            // For now, do not allow dragging on top of another section since we don't handle multiple sections in one
            // tile.
            if (_mutableMapSectionData.TileMetadataMap.ContainsKey(tileCoords) &&
                _mutableMapSectionData.TileMetadataMap[tileCoords].SectionConnection != null) {
                return;
            }

            if (_mutableMapSectionData.TileMetadataMap[_tileCoords].SectionConnection == null) {
                throw new Exception("Section connection not found in SectionTileMapElement");
            }
            
            uint? previousConnection = _mutableMapSectionData.TileMetadataMap[_tileCoords].SectionConnection;
            if (previousConnection != null) {
                _mutableMapSectionData.ClearSectionConnection(_tileCoords);
            }
            
            _mutableMapSectionData.SetSectionConnection(tileCoords, previousConnection.Value);
            _tileCoords = tileCoords;
        }

        public void Remove() {
            _mutableMapSectionData.ClearSectionConnection(_tileCoords);
        }
    }
}