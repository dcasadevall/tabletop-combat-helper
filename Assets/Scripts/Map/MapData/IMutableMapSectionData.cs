using Math;

namespace Map.MapData {
    public interface IMutableMapSectionData : IMapSectionData {
        /// <summary>
        /// Adds or modifies a section connection to the given tile coords, into the given section index.
        /// </summary>
        /// <param name="tileCoords"></param>
        /// <param name="sectionIndex"></param>
        void SetSectionConnection(IntVector2 tileCoords, uint sectionIndex);
        /// <summary>
        /// Removes (if possible) a section connection in the given tile coords.
        /// </summary>
        /// <param name="tileCoords"></param>
        void ClearSectionConnection(IntVector2 tileCoords);
    }
}