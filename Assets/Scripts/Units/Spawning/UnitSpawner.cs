using CommandSystem;
using EncounterSelection;
using Grid;
using Grid.Positioning;
using Map.MapSections;
using Math;
using UniRx;
using Units.Serialized;
using Units.Spawning.Commands;
using Units.UI;
using UnityEngine;
using Zenject;

namespace Units.Spawning {
    /// <summary>
    /// Handles Spawning units in the world space when the scene context is first loaded.
    /// It uses the <see cref="IUnitPool"/> in order to spawn such units.
    /// </summary>
    public class UnitSpawner : IInitializable, ITickable {
        private readonly bool _shouldSpawnInitialUnits;
        private readonly IUnitPickerViewController _unitPickerViewController;
        private readonly IGridPositionCalculator _gridPositionCalculator;
        private readonly IRandomGridPositionProvider _randomGridPositionProvider;
        private readonly IUnitSpawnSettings _unitSpawnSettings;
        private readonly IEncounterSelectionContext _encounterSelectionContext;
        private readonly IGridInputManager _gridInputManager;
        private readonly IFactory<IUnitData, UnitCommandData> _unitCommandDataFactory;
        private readonly ICommandQueue _commandQueue;
        private IntVector2? _selectedTile;

        public UnitSpawner(IEncounterSelectionContext encounterSelectionContext,
                           IUnitPickerViewController unitPickerVc, 
                           IGridPositionCalculator gridPositionCalculator,
                           IRandomGridPositionProvider randomGridPositionProvider,
                           IGridInputManager gridInputManager,
                           IFactory<IUnitData, UnitCommandData> unitCommandDataFactory,
                           ICommandQueue commandQueue,
                           IUnitSpawnSettings unitSpawnSettings) {
            _encounterSelectionContext = encounterSelectionContext;
            _gridInputManager = gridInputManager;
            _unitCommandDataFactory = unitCommandDataFactory;
            _commandQueue = commandQueue;
            _randomGridPositionProvider = randomGridPositionProvider;
            _unitPickerViewController = unitPickerVc;
            _gridPositionCalculator = gridPositionCalculator;
            _unitSpawnSettings = unitSpawnSettings;
        }

        // TODO: This is really janky. We should find a better way to track unit spawn as initial commands.
        // How about not spawn units with the spawner on subsequent floors?
        private static bool _firstSpawn = false;
        public void Initialize() {
            _unitPickerViewController.SpawnUnitClicked += HandleSpawnUnitClicked;

            // If we are loading a replay, don't attempt to initially spawn units
            if (_encounterSelectionContext.EncounterType == EncounterType.Replay) {
                return;
            } 
            
            // Spawn initial player units
            IUnitData[] playerUnits = _unitSpawnSettings.GetUnits(UnitType.Player);
            IntVector2 startPosition = _gridPositionCalculator.GetTileClosestToCenter();
            IntVector2[] tilePositions =
                _randomGridPositionProvider.GetRandomUniquePositions(startPosition,
                                                                     _unitSpawnSettings
                                                                         .MaxInitialUnitSpawnDistanceToCenter,
                                                                     playerUnits.Length);
            for (int i = 0; i < tilePositions.Length; i++) {
                SpawnUnit(playerUnits[i], tilePositions[i]);
            }

            _firstSpawn = true;
        }

        public void Tick() {
            if (Input.GetKeyUp(KeyCode.U)) {
                _selectedTile = _gridInputManager.GetTileAtMousePosition();
                _unitPickerViewController.Show();
            }
        }

        private void HandleSpawnUnitClicked(IUnitData unitData, int numUnits) {
            _unitPickerViewController.Hide();
            IntVector2[] tilePositions =
                _randomGridPositionProvider.GetRandomUniquePositions(_selectedTile ?? IntVector2.Zero,
                                                                     1,
                                                                     numUnits);
           
            foreach (var tilePosition in tilePositions) {
                SpawnUnit(unitData, tilePosition);
            }
        }

        private void SpawnUnit(IUnitData unitData, IntVector2 tileCoords) {
            Debug.Log("Spawner Hash: " + GetHashCode());
            UnitCommandData unitCommandData = _unitCommandDataFactory.Create(unitData);
            SpawnUnitData spawnUnitData = new SpawnUnitData(unitCommandData, tileCoords, _firstSpawn);
            _commandQueue.Enqueue<SpawnUnitCommand, SpawnUnitData>(spawnUnitData, CommandSource.Game);
        }
    }
}