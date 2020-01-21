namespace Map.MapData {
    public interface IMapData {
        string MapName { get; }
        IMapSectionData[] Sections { get; }
    }
}