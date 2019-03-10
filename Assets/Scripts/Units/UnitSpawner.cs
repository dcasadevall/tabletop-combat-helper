using System;
using System.Collections.Generic;
using Grid;
using Grid.Positioning;
using Logging;
using Units.Serialized;
using Units.UI;
using UnityEngine;
using UnityEngine.Networking;
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
        private IGridPositionCalculator _gridPositionCalculator;
        private IUnitSpawnSettings _unitSpawnSettings;
        private IGridUnitManager _gridUnitManager;
        private ILogger _logger;
        private UnitBehaviour.Pool _unitBehaviourPool;

        public UnitSpawner(IUnitPickerViewController unitPickerVc, 
                           IGrid grid,
                           IGridPositionCalculator gridPositionCalculator,
                           IRandomGridPositionProvider randomGridPositionProvider,
                           IGridUnitManager gridUnitManager,
                           IUnitSpawnSettings unitSpawnSettings,
                           ILogger logger, 
                           UnitBehaviour.Pool unitBehaviourPool) {
            _logger = logger;
            _grid = grid;
            _gridPositionCalculator = gridPositionCalculator;
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
            UnitBehaviour unitBehaviour = _unitBehaviourPool.Spawn(unitData);
            
            // Temp code to set position. In the future, we will start the unit in the grid.
            Vector2 worldPosition =
                _gridPositionCalculator.GetTileCenterWorldPosition(_grid, (int) tilePosition.x, (int) tilePosition.y);
            unitBehaviour.transform.position =
                new Vector3(worldPosition.x, worldPosition.y, unitBehaviour.transform.position.z);

            IUnit unit = new Unit(new UnitId(), unitData);
            _gridUnitManager.PlaceUnitAtTile(unit.UnitId, (int) tilePosition.x, (int) tilePosition.y);
            NetworkServer.Spawn(unitBehaviour.gameObject);
        }
    }
}