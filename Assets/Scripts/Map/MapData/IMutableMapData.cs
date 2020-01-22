namespace Map.MapData {
    public interface IMutableMapData : IMapData {
        new IMutableMapSectionData[] Sections { get; }
    }
}