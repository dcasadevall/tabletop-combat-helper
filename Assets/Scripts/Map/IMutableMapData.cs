namespace Map {
    public interface IMutableMapData : IMapData {
        IMutableMapSectionData[] Sections { get; }
    }
}