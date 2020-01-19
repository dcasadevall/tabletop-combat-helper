using UniRx.Async;

namespace Map {
    /// <summary>
    /// <see cref="IMapReference"/> that can also be used to load the asset for the given map.
    /// It should only be injected to the class loading the maps initially.
    /// </summary>
    public interface ILoadadableMapReference : IMapReference {
        /// <summary>
        /// Attempts to load the map asynchronously, returning the loaded <see cref="IMutableMapData"/> on success.
        /// </summary>
        /// <returns></returns>
        UniTask<IMutableMapData> LoadMap();
    }
}