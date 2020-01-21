using System.Collections.Generic;
using UniRx.Async;

namespace Map.MapData.Store {
    /// <summary>
    /// A version of the Map Asset store that allows mutating / committing the loaded assets.
    /// Should only be used in map editor mode.
    /// </summary>
    public interface IReadWriteMapAssetStore {
        /// <summary>
        /// List of <see cref="IMapReference"/>s available to load. 
        /// </summary>
        List<IMapReference> MapReferences { get; }

        /// <summary>
        /// Attempts to load the given <see cref="MapStoreId"/> asynchronously,
        /// returning the loaded <see cref="IMapAsset"/> on success.
        /// 
        /// Note that changes to the returned data should be persisted by calling <see cref="Commit()"/>.
        /// </summary>
        /// <returns></returns>
        UniTask<IMutableMapAsset> LoadMap(MapStoreId mapStoreId);

        /// <summary>
        /// Attempts to commit the changes made to the <see cref="IMutableMapAsset"/>. 
        /// </summary>
        /// <returns>True if successfully committed the changes. False otherwise.</returns>
        bool Commit();
    }
}