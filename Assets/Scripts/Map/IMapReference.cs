using System;
using Map.Serialized;
using UniRx.Async;
using UnityEngine.AddressableAssets;

namespace Map {
    /// <summary>
    /// Reference displayed in the map selection.
    /// This reference should be accessible WITHOUT loading the map.
    /// We use this indirection so we can avoid loading map data (and its assets) unless we need it.
    /// <see cref="ILoadadableMapReference"/> will be injected to the actors which need to actually load the map.
    /// </summary>
    public interface IMapReference {
        /// <summary>
        /// Map name to be listed in the map selection list.
        /// </summary>
        string MapName { get; }
    }
}