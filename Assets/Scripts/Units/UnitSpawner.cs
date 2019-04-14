using System;
using System.Collections.Generic;
using CommandSystem;
using Grid;
using Grid.Positioning;
using Logging;
using Math;
using Units.Commands;
using Units.Serialized;
using Units.UI;
using UnityEngine;
using Zenject;
using ILogger = Logging.ILogger;

namespace Units {
    /// <summary>
    /// Handles Spawning units in the world space.
    /// It uses the <see cref="UnitBehaviour.Pool"/> in order to spawn such units.
    ///
    /// <see cref="UnitBehaviour"/>s are initialized with the newly created <see cref="IUnit"/>.
    /// </summary>
    public class UnitSpawner : IInitializable, ITickable {
        private readonly IUnitPickerViewController _unitPickerViewController;
        private readonly IGridPositionCalculator _gridPositionCalculator;
        private readonly IRandomGridPositionProvider _randomGridPositionProvider;
        private readonly IUnitSpawnSettings _unitSpawnSettings;
        private readonly ILogger _logger;
        private readonly IGridInputManager _gridInputManager;
        private readonly IFactory<IUnitData, UnitCommandData> _unitCommandDataFactory;
        private readonly ICommandQueue _commandQueue;
        private IntVector2? _selectedTile;

        public UnitSpawner(IUnitPickerViewController unitPickerVc, 
                           IGridPositionCalculator gridPositionCalculator,
                           IRandomGridPositionProvider randomGridPositionProvider,
                           IGridInputManager gridInputManager,
                           IFactory<IUnitData, UnitCommandData> unitCommandDataFactory,
                           ICommandQueue commandQueue,
                           IUnitSpawnSettings unitSpawnSettings,
                           ILogger logger) {
            _gridInputManager = gridInputManager;
            _unitCommandDataFactory = unitCommandDataFactory;
            _commandQueue = commandQueue;
            _randomGridPositionProvider = randomGridPositionProvider;
            _unitPickerViewController = unitPickerVc;
            _gridPositionCalculator = gridPositionCalculator;
            _unitSpawnSettings = unitSpawnSettings;
            _logger = logger;
        }

        public void Initialize() {
            _unitPickerViewController.SpawnUnitClicked += HandleSpawnUnitClicked;

            // Spawn 
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
            UnitCommandData unitCommandData = _unitCommandDataFactory.Create(unitData);
            SpawnUnitData spawnUnitData = new SpawnUnitData(unitCommandData, tileCoords);
            _commandQueue.Enqueue<SpawnUnitCommand, SpawnUnitData>(spawnUnitData);
        }
    }
}