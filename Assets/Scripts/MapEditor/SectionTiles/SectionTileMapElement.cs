using Map.MapData;
using MapEditor.MapElement;
using Math;

namespace MapEditor.SectionTiles {
    public class SectionTileMapElement : IMapElement {
        private readonly IMutableMapSectionData _mutableMapSectionData;
        private readonly IntVector2 _tileCoords;

        public SectionTileMapElement(IMutableMapSectionData mutableMapSectionData, IntVector2 tileCoords) {
            _mutableMapSectionData = mutableMapSectionData;
            _tileCoords = tileCoords;
        }

        public void Remove() {
            _mutableMapSectionData.ClearSectionConnection(_tileCoords);
        }
    }
}