using Map.Serialized;
using UniRx.Async;

namespace Map {
    public interface IMutableMapReference {
        /// <summary>
        /// Attempts to load the map asynchronously, returning the mutable map data information.
        /// </summary>
        /// <returns></returns>
        UniTask<MapData> LoadMap(); 
    }
}