using System.Collections.Generic;
using UniRx.Async;

namespace Map.MapData.Store {
    /// <summary>
    /// Datastore that can be used to Load the Map Assets for each level.
    /// 
    /// This store does not allow asset modification / saving. For that, you should use
    /// <see cref="IReadWriteMapAssetStore"/>.
    /// </summary>
    public interface IReadOnlyMapAssetStore {
        /// <summary>
        /// List of <see cref="IMapReference"/>s available to load. 
        /// </summary>
        List<IMapReference> MapReferences { get; }

        /// <summary>
        /// Attempts to load the given <see cref="MapStoreId"/> asynchronously,
        /// returning the loaded <see cref="IMapAsset"/> on success.
        /// </summary>
        /// <returns></returns>
        UniTask<IMapAsset> LoadMap(MapStoreId mapStoreId);
    }
}