using Map.MapData;
using MapEditor.MapElement;
using Math;

namespace MapEditor.PlayerUnits {
    public class PlayerUnitsMapElement : IMapElement {
        private readonly IMutableMapSectionData _mapSectionData;

        public PlayerUnitsMapElement(IMutableMapSectionData mapSectionData) {
            _mapSectionData = mapSectionData;
        }

        public void HandleDrag(IntVector2 tileCoords) {
            _mapSectionData.SetPlayerUnitSpawnPoint(tileCoords);
        }

        public void Remove() {
            _mapSectionData.ClearPlayerUnitSpawnPoint();
        }
    }
}