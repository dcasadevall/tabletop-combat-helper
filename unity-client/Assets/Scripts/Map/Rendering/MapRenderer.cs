using UnityEngine;
using Zenject;

namespace Map.Rendering {
    /// <summary>
    /// <see cref="IInitializable"/> object used to initially render the map and it's tiles.
    /// </summary>
    public class MapRenderer : IInitializable {
        private readonly ITileLoader _tileLoader;
        private readonly MapBehaviour.Factory _mapFactory;

        public MapRenderer(ITileLoader tileLoader, MapBehaviour.Factory mapFactory) {
            _tileLoader = tileLoader;
            _mapFactory = mapFactory;
        }
        
        public void Initialize() {
            _tileLoader.LoadTiles();
            _mapFactory.Create();
        }
    }
}