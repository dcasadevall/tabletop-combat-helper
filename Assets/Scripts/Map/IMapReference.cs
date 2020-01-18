using System;
using UnityEngine.AddressableAssets;

namespace Map {
    /// <summary>
    /// Reference displayed in the map selection, as well as the actual map data asset to be loaded.
    /// This reference should be accessible WITHOUT loading the map.
    /// We use this indirection so we can avoid loading map data (and its assets) unless we need it.
    /// </summary>
    public interface IMapReference {
        /// <summary>
        /// Map name to be listed in the map selection list.
        /// </summary>
        string MapName { get; }
        
        /// <summary>
        /// Attempts to load the map asynchronously.
        /// </summary>
        /// <returns></returns>
        IObservable<IMapData> LoadMap();
    }
}