using System.ComponentModel;
using Zenject;

namespace Map.Rendering {
    public class MapRendererFactory : IFactory<IMapRenderer> {
        private readonly IMapData _mapData;
        private readonly RandomizedRepeatedTilesMapRenderer _randomizedRepeatedTilesMapRenderer;
        private readonly SequentialUniqueTilesMapRenderer _sequentialUniqueTilesMapRenderer;

        public MapRendererFactory([Inject(Id = "LoadedMap")]
                                  IMapData mapData,
                                  RandomizedRepeatedTilesMapRenderer randomizedRepeatedTilesMapRenderer,
                                  SequentialUniqueTilesMapRenderer sequentialUniqueTilesMapRenderer) {
            _mapData = mapData;
            _randomizedRepeatedTilesMapRenderer = randomizedRepeatedTilesMapRenderer;
            _sequentialUniqueTilesMapRenderer = sequentialUniqueTilesMapRenderer;
        }
        
        public IMapRenderer Create() {
            switch (_mapData.MapTileType) {
                case MapTileType.RandomizeRepeatedTiles:
                    return _randomizedRepeatedTilesMapRenderer;
                case MapTileType.SequentialUniqueTiles:
                    return _sequentialUniqueTilesMapRenderer;
                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }
}