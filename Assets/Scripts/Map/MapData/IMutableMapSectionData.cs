using Math;
using Units.Serialized;

namespace Map.MapData {
    public interface IMutableMapSectionData : IMapSectionData {
        /// <summary>
        /// Adds a unit to be spawned initially when the map loads in the given coords.
        /// </summary>
        /// <param name="tileCoords"></param>
        /// <param name="unit"></param>
        void AddInitialUnit(IntVector2 tileCoords, IUnitData unit);
        /// <summary>
        /// Removes a unit to be spawned initially  when the map loads in the given coords.
        /// </summary>
        /// <param name="tileCoords"></param>
        /// <param name="unit"></param>
        void RemoveInitialUnit(IntVector2 tileCoords, IUnitData unit);
        /// <summary>
        /// Sets the coordinates where the player units will spawn when the map is loaded.
        /// </summary>
        /// <param name="spawnPoint"></param>
        void SetPlayerUnitSpawnPoint(IntVector2 spawnPoint);
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