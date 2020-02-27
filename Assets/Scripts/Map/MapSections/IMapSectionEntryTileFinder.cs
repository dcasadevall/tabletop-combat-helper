using Math;

namespace Map.MapSections {
    public interface IMapSectionEntryTileFinder {
        IntVector2 GetEntryTile(uint sectionIndex, uint fromSectionIndex);
    }
}