namespace Map.MapData.Store {
    public interface IMapAsset {
        /// <summary>
        /// Returns the loaded <see cref="IMapData"/>.
        /// This data is not mutable.
        /// </summary>
        IMapData MapData { get; }
    }
}