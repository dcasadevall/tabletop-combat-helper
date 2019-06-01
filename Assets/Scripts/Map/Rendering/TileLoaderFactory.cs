using System.ComponentModel;
using Map.MapSections;
using Zenject;

namespace Map.Rendering {
    /// <summary>
    /// A factory used to choose the <see cref="ITileLoader"/> based on the current <see cref="MapTileType"/> in
    /// context.
    /// </summary>
    public class TileLoaderFactory : IFactory<ITileLoader> {
        private readonly IMapSectionData _mapSectionData;
        private readonly RandomizedRepeatedTileLoader _randomizedRepeatedTileLoader;
        private readonly SequentialUniqueTileLoader _sequentialUniqueTileLoader;

        public TileLoaderFactory(IMapSectionData mapSectionData,
                                 RandomizedRepeatedTileLoader randomizedRepeatedTileLoader,
                                 SequentialUniqueTileLoader sequentialUniqueTileLoader) {
            _mapSectionData = mapSectionData;
            _randomizedRepeatedTileLoader = randomizedRepeatedTileLoader;
            _sequentialUniqueTileLoader = sequentialUniqueTileLoader;
        }
        
        public ITileLoader Create() {
            switch (_mapSectionData.MapTileType) {
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