namespace Map.Rendering {
    public interface IMapRendererFactory {
        ITileLoader createMapRenderer(MapTileType mapTileType);
    }
}