using CommandSystem;
using EncounterSelection;
using Grid.Positioning;
using Map.MapData;
using Math;
using Units.Serialized;
using Units.Spawning.Commands;
using Zenject;

namespace Units.Spawning {
    /// <summary>
    /// Handles Spawning player units in the world space when the scene context is first loaded.
    /// </summary>
    public class PlayerUnitSpawner : IInitializable {
        private readonly bool _shouldSpawnInitialUnits;
        private readonly IRandomGridPositionProvider _randomGridPositionProvider;
        private readonly IUnitSpawnSettings _unitSpawnSettings;
        private readonly IEncounterSelectionContext _encounterSelectionContext;
        private readonly IFactory<IUnitData, UnitCommandData> _unitCommandDataFactory;
        private readonly IMapSectionData _mapSectionData;
        private readonly ICommandQueue _commandQueue;

        public PlayerUnitSpawner(IEncounterSelectionContext encounterSelectionContext,
                                 IRandomGridPositionProvider randomGridPositionProvider,
                                 IFactory<IUnitData, UnitCommandData> unitCommandDataFactory,
                                 IMapSectionData mapSectionData,
                                 ICommandQueue commandQueue,
                                 IUnitSpawnSettings unitSpawnSettings) {
            _encounterSelectionContext = encounterSelectionContext;
            _unitCommandDataFactory = unitCommandDataFactory;
            _mapSectionData = mapSectionData;
            _commandQueue = commandQueue;
            _randomGridPositionProvider = randomGridPositionProvider;
            _unitSpawnSettings = unitSpawnSettings;
        }

        public void Initialize() {
            // If we are loading a replay or edit mode, don't attempt to initially spawn units
            if (_encounterSelectionContext.EncounterType != EncounterType.Combat) {
                return;
            }

            if (_mapSectionData.PlayerUnitSpawnPoint == null) {
                return;
            }

            // Spawn initial player units
            IUnitData[] playerUnits = _unitSpawnSettings.GetUnits(UnitType.Player);
            IntVector2 startPosition = _mapSectionData.PlayerUnitSpawnPoint.Value;
            IntVector2[] tilePositions =
                _randomGridPositionProvider.GetRandomUniquePositions(startPosition,
                                                                     _unitSpawnSettings
                                                                         .MaxInitialUnitSpawnDistanceToCenter,
                                                                     playerUnits.Length);
            for (int i = 0; i < tilePositions.Length; i++) {
                SpawnUnit(playerUnits[i], tilePositions[i]);
            }
        }

        private void SpawnUnit(IUnitData unitData, IntVector2 tileCoords) {
            UnitCommandData unitCommandData = _unitCommandDataFactory.Create(unitData);
            SpawnUnitData spawnUnitData = new SpawnUnitData(unitCommandData, tileCoords, isInitialSpawn: true);
            _commandQueue.Enqueue<SpawnUnitCommand, SpawnUnitData>(spawnUnitData, CommandSource.Game);
        }
    }
}