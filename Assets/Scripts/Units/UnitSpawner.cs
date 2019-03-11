using System;
using System.Collections.Generic;
using Grid;
using Grid.Positioning;
using Logging;
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
        private IUnitPickerViewController _unitPickerViewController;
        private IRandomGridPositionProvider _randomGridPositionProvider;
        private IGrid _grid;
        private IUnitSpawnSettings _unitSpawnSettings;
        private IGridUnitManager _gridUnitManager;
        private ILogger _logger;
        private UnitBehaviour.Pool _unitBehaviourPool;

        public UnitSpawner(IUnitPickerViewController unitPickerVc, 
                           IGrid grid,
                           IRandomGridPositionProvider randomGridPositionProvider,
                           IGridUnitManager gridUnitManager,
                           IUnitSpawnSettings unitSpawnSettings,
                           ILogger logger, 
                           UnitBehaviour.Pool unitBehaviourPool) {
            _logger = logger;
            _grid = grid;
            _gridUnitManager = gridUnitManager;
            _randomGridPositionProvider = randomGridPositionProvider;
            _unitPickerViewController = unitPickerVc;
            _unitBehaviourPool = unitBehaviourPool;
            _unitSpawnSettings = unitSpawnSettings;
        }

        public void Initialize() {
            _unitPickerViewController.SpawnUnitClicked += HandleSpawnUnitClicked;

            // Spawn 
            Vector2[] tilePositions =
                _randomGridPositionProvider.GetRandomUniquePositions(_grid,
                                                                     _unitSpawnSettings
                                                                         .MaxInitialUnitSpawnDistanceToCenter,
                                                                     _unitSpawnSettings.PlayerUnits.Length);
            for (int i = 0; i < tilePositions.Length; i++) {
                SpawnUnit(_unitSpawnSettings.PlayerUnits[i], tilePositions[i]);
            }
        }

        public void Tick() {
            if (Input.GetKeyUp(KeyCode.Space)) {
                _unitPickerViewController.Show();
            }
        }

        private void HandleSpawnUnitClicked(IUnitData unitData) {
            _unitPickerViewController.Hide();
            SpawnUnit(unitData, Vector3.zero);
        }

        private void SpawnUnit(IUnitData unitData, Vector2 tilePosition) {
            _logger.Log(LoggedFeature.Units, "Spawning: {0}", unitData.Name);

            IUnit unit = new Unit(unitData);
            foreach (var unitInHierarchy in unit.GetUnitsInHierarchy()) {
                _unitBehaviourPool.Spawn(unitInHierarchy);
                _gridUnitManager.PlaceUnitAtTile(unitInHierarchy, (int) tilePosition.x, (int) tilePosition.y);
            }
        }
    }
}