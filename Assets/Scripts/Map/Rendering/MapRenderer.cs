using Zenject;

namespace Map.Rendering {
    public class MapRenderer : IInitializable {
        private readonly IMapData _mapData;
        private readonly ITileLoader _tileLoader;
        private readonly MapBehaviour.Factory _mapFactory;

        public MapRenderer(IMapData mapData, ITileLoader tileLoader, MapBehaviour.Factory mapFactory) {
            _mapData = mapData;
            _tileLoader = tileLoader;
            _mapFactory = mapFactory;
        }
        
        public void Initialize() {
            _tileLoader.LoadTiles(_mapData);
            _mapFactory.Create(_mapData);
        }
    }
}