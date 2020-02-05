using CommandSystem;
using EncounterSelection;
using Logging;
using Map.MapData;
using Math;
using Units.Serialized;
using Units.Spawning.Commands;
using Zenject;

namespace Units.Spawning {
    public class InitialUnitSpawner : IInitializable {
        private readonly IFactory<IUnitData, UnitCommandData> _unitCommandDataFactory;
        private readonly ICommandQueue _commandQueue;
        private readonly IMapSectionData _mapSectionData;
        private readonly IUnitDataIndexResolver _unitDataIndexResolver;
        private readonly ILogger _logger;

        public InitialUnitSpawner(IFactory<IUnitData, UnitCommandData> unitCommandDataFactory,
                                  ICommandQueue commandQueue,
                                  IMapSectionData mapSectionData,
                                  IUnitDataIndexResolver unitDataIndexResolver,
                                  ILogger logger) {
            _unitCommandDataFactory = unitCommandDataFactory;
            _commandQueue = commandQueue;
            _mapSectionData = mapSectionData;
            _unitDataIndexResolver = unitDataIndexResolver;
            _logger = logger;
        }

        public void Initialize() {
            foreach (var tileMetadataKvp in _mapSectionData.TileMetadataMap) {
                foreach (var unit in tileMetadataKvp.Value.Units) {
                    SpawnUnit(unit, tileMetadataKvp.Key);
                }
            }
        }

        private void SpawnUnit(UnitDataReference unitDataReference, IntVector2 tileCoords) {
            IUnitData unitData =
                _unitDataIndexResolver.ResolveUnitData(unitDataReference.UnitType, unitDataReference.UnitIndex);
            if (unitData == null) {
                _logger.LogError(LoggedFeature.Units, "UnitData reference not found: {0}", unitDataReference);
                return;
            }
            
            UnitCommandData unitCommandData = _unitCommandDataFactory.Create(unitData);
            SpawnUnitData spawnUnitData = new SpawnUnitData(unitCommandData, tileCoords, isInitialSpawn: true);
            _commandQueue.Enqueue<SpawnUnitCommand, SpawnUnitData>(spawnUnitData, CommandSource.Game);
        }
    }
}