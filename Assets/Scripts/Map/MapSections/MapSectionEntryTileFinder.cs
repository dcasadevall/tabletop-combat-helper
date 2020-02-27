using System.Linq;
using Logging;
using Map.MapData;
using Map.MapData.TileMetadata;
using Math;

namespace Map.MapSections {
    public class MapSectionEntryTileFinder : IMapSectionEntryTileFinder {
        private readonly IMapData _mapData;
        private readonly ILogger _logger;

        public MapSectionEntryTileFinder(IMapData mapData, ILogger logger) {
            _mapData = mapData;
            _logger = logger;
        }

        public IntVector2 GetEntryTile(uint sectionIndex, uint fromSectionIndex) {
            if (_mapData.Sections.Length <= sectionIndex) {
                _logger.LogError(LoggedFeature.Map, "Section index not in map: {0}", sectionIndex);
                return default;
            }

            IMapSectionData mapSectionData = _mapData.Sections[sectionIndex];
            var kvp =
                mapSectionData.TileMetadataMap
                              .FirstOrDefault(x => x.Value.SectionConnection != null &&
                                                   x.Value.SectionConnection.Value == fromSectionIndex);
            if (kvp.Value == null) {
                _logger.Log(LoggedFeature.Map, "No section connections found: {0}", sectionIndex);
                return default;
            }

            return kvp.Key;
        }
    }
}