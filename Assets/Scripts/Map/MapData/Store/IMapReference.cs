namespace Map.MapData.Store {
    /// <summary>
    /// Reference displayed in the map selection.
    /// This reference should be accessible WITHOUT loading the map.
    /// We use this indirection so we can avoid loading map data (and its assets) unless we need it.
    /// <see cref="IReadOnlyMapAssetStore"/> will be injected to the actors which need to actually load / save the map data.
    /// </summary>
    public interface IMapReference {
        /// <summary>
        /// <see cref="MapStoreId"/> of the map being referenced.
        /// </summary>
        MapStoreId MapStoreId { get; }
        /// <summary>
        /// Map name to be listed in the map selection list.
        /// </summary>
        string MapName { get; }
    }
}