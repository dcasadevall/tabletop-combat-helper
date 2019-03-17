namespace Map.Rendering {
    public interface IMapRendererFactory {
        IMapRenderer createMapRenderer(MapTileType mapTileType);
    }
}