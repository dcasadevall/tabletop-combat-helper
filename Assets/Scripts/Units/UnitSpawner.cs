using System;
using System.Collections.Generic;
using Grid;
using Grid.Positioning;
using Logging;
using Math;
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
        private readonly IGridUnitManager _gridUnitManager;
        private readonly IGridInputManager _gridInputManager;
        private readonly ILogger _logger;
        private readonly UnitBehaviour.Pool _unitBehaviourPool;
        private IntVector2? _selectedTile;

        public UnitSpawner(IUnitPickerViewController unitPickerVc, 
                           IGridPositionCalculator gridPositionCalculator,
                           IRandomGridPositionProvider randomGridPositionProvider,
                           IGridUnitManager gridUnitManager,
                           IGridInputManager gridInputManager,
                           IUnitSpawnSettings unitSpawnSettings,
                           ILogger logger, 
                           UnitBehaviour.Pool unitBehaviourPool) {
            _logger = logger;
            _gridUnitManager = gridUnitManager;
            _gridInputManager = gridInputManager;
            _randomGridPositionProvider = randomGridPositionProvider;
            _unitPickerViewController = unitPickerVc;
            _gridPositionCalculator = gridPositionCalculator;
            _unitBehaviourPool = unitBehaviourPool;
            _unitSpawnSettings = unitSpawnSettings;
        }

        public void Initialize() {
            _unitPickerViewController.SpawnUnitClicked += HandleSpawnUnitClicked;

            // Spawn 
            IntVector2 startPosition = _gridPositionCalculator.GetTileClosestToCenter();
            IntVector2[] tilePositions =
                _randomGridPositionProvider.GetRandomUniquePositions(startPosition,
                                                                     _unitSpawnSettings
                                                                         .MaxInitialUnitSpawnDistanceToCenter,
                                                                     _unitSpawnSettings.PlayerUnits.Length);
            for (int i = 0; i < tilePositions.Length; i++) {
                SpawnUnit(_unitSpawnSettings.PlayerUnits[i], tilePositions[i]);
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
            _logger.Log(LoggedFeature.Units, "Spawning: {0}", unitData.Name);

            IUnit unit = new Unit(unitData);
            foreach (var unitInHierarchy in unit.GetUnitsInHierarchy()) {
                _unitBehaviourPool.Spawn(unitInHierarchy);
                _gridUnitManager.PlaceUnitAtTile(unitInHierarchy, tileCoords);
            }
        }
    }
}