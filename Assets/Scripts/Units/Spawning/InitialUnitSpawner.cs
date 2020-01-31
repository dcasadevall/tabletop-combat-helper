using CommandSystem;
using EncounterSelection;
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

        public InitialUnitSpawner(IFactory<IUnitData, UnitCommandData> unitCommandDataFactory,
                                  ICommandQueue commandQueue,
                                  IMapSectionData mapSectionData) {
            _unitCommandDataFactory = unitCommandDataFactory;
            _commandQueue = commandQueue;
            _mapSectionData = mapSectionData;
        }

        public void Initialize() {
            foreach (var tileMetadataKvp in _mapSectionData.TileMetadataMap) {
                foreach (var unit in tileMetadataKvp.Value.Units) {
                    SpawnUnit(unit, tileMetadataKvp.Key);
                }
            }
        }

        private void SpawnUnit(IUnitData unitData, IntVector2 tileCoords) {
            UnitCommandData unitCommandData = _unitCommandDataFactory.Create(unitData);
            SpawnUnitData spawnUnitData = new SpawnUnitData(unitCommandData, tileCoords, isInitialSpawn: true);
            _commandQueue.Enqueue<SpawnUnitCommand, SpawnUnitData>(spawnUnitData, CommandSource.Game);
        }
    }
}