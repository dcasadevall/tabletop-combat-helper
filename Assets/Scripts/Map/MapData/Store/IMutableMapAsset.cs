namespace Map.MapData.Store {
    public interface IMutableMapAsset {
        /// <summary>
        /// Returns the loaded <see cref="IMutableMapData"/>.
        /// This data can be modified and committed (via <see cref="IReadWriteMapAssetStore.Commit()"/>.
        /// </summary>
        IMutableMapData MapData { get; }
    }
}