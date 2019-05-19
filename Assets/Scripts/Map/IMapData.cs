namespace Map {
    public interface IMapData {
        string MapName { get; }
        IMapSectionData[] Sections { get; }
    }
}