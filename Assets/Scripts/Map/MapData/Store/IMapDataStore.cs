using System.Collections.Generic;
using UniRx.Async;

namespace Map.MapData.Store {
    /// <summary>
    /// Data Store used to load / save map data.
    /// Implementations may choose to restrict saving the mutated map data.
    /// </summary>
    public interface IMapDataStore {
        /// <summary>
        /// List of <see cref="IMapReference"/>s available to load. 
        /// </summary>
        List<IMapReference> MapReferences { get; }

        /// <summary>
        /// Attempts to load the given <see cref="MapStoreId"/> asynchronously,
        /// returning the loaded <see cref="IMutableMapData"/> on success.
        /// 
        /// Note that changes to the returned data should be persisted by calling <see cref="Commit()"/>.
        /// </summary>
        /// <returns></returns>
        UniTask<IMutableMapData> LoadMap(MapStoreId mapStoreId);

        /// <summary>
        /// Attempts to commit the changes made to the <see cref="IMutableMapData"/> with the given <see cref="MapStoreId"/>.
        /// This method may be unsupported by implementations of <see cref="IMapDataStore"/>
        /// </summary>
        /// <returns>True if successfully committed the changes. False otherwise.</returns>
        bool Commit(MapStoreId mapStoreId);
    }
}