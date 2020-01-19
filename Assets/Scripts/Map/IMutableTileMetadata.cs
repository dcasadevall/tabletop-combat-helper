namespace Map {
    public interface IMutableTileMetadata : ITileMetadata {
        uint? SectionConnection { set; }
    }
}