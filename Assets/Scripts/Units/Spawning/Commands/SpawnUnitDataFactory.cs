using Logging;
using Math;
using Units.Serialized;

namespace Units.Spawning.Commands {
    public class SpawnUnitDataFactory {
        private readonly IUnitDataIndexResolver _indexResolver;
        private readonly ILogger _logger;

        public SpawnUnitDataFactory(IUnitDataIndexResolver indexResolver, ILogger logger) {
            _indexResolver = indexResolver;
            _logger = logger;
        }

        /// <summary>
        /// Creates a <see cref="SpawnUnitData"/> object from the given <see cref="IUnitData"/>.
        /// </summary>
        /// <param name="unitData"></param>
        /// <param name="tileCoords"></param>
        /// <returns>The created <see cref="SpawnUnitData"/>, or null if it could not be created.</returns>
        public SpawnUnitData OfUnitData(IUnitData unitData, IntVector2 tileCoords) {
            uint? index = _indexResolver.ResolveUnitIndex(unitData);
            if (index == null) {
                _logger.LogError(LoggedFeature.Units, $"Unit index not resolved: {unitData.Name}");
                return null;
            }

            int i = 0;
            UnitCommandData[] petCommandDatas = new UnitCommandData[unitData.Pets.Length];
            foreach (IUnitData petData in unitData.Pets) {
                petCommandDatas[i] = OfUnitData(petData, tileCoords);
            }
            
            UnitCommandData unitCommandData = new UnitCommandData();
            SpawnUnitData spawnUnitData = new SpawnUnitData();
        }
    }
}