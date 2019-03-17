using System.ComponentModel;
using Zenject;

namespace Map.Rendering {
    /// <summary>
    /// A factory used to choose the <see cref="ITileLoader"/> based on the current <see cref="MapTileType"/> in
    /// context.
    /// </summary>
    public class TileLoaderFactory : IFactory<ITileLoader> {
        private readonly IMapData _mapData;
        private readonly RandomizedRepeatedTileLoader _randomizedRepeatedTileLoader;
        private readonly SequentialUniqueTileLoader _sequentialUniqueTileLoader;

        public TileLoaderFactory(IMapData mapData,
                                 RandomizedRepeatedTileLoader randomizedRepeatedTileLoader,
                                 SequentialUniqueTileLoader sequentialUniqueTileLoader) {
            _mapData = mapData;
            _randomizedRepeatedTileLoader = randomizedRepeatedTileLoader;
            _sequentialUniqueTileLoader = sequentialUniqueTileLoader;
        }
        
        public ITileLoader Create() {
            switch (_mapData.MapTileType) {
                case MapTileType.RandomizeRepeatedTiles:
                    return _randomizedRepeatedTileLoader;
                case MapTileType.SequentialUniqueTiles:
                    return _sequentialUniqueTileLoader;
                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }
}