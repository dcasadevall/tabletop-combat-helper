using System.Collections.Generic;
using Logging;
using Units.Serialized;
using Units.UI;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;
using ILogger = Logging.ILogger;

namespace Prototype {
    // TEMPORARY prototype class to spawn units.
    public class UnitSpawner : IInitializable, ITickable {
        private IUnitPickerViewController _unitPickerViewController;
        private ILogger _logger;
        private List<IUnitData> _unitDatas;
        private UnitNetworkBehaviour.Pool _unitNetworkBehaviourPool;

        public UnitSpawner(IUnitPickerViewController unitPickerVc, 
                           ILogger logger, 
                           List<IUnitData> unitDatas,
                           UnitNetworkBehaviour.Pool unitNetworkBehaviourPool) {
            _logger = logger;
            _unitPickerViewController = unitPickerVc;
            _unitDatas = unitDatas;
            _unitNetworkBehaviourPool = unitNetworkBehaviourPool;
        }

        public void Initialize() {
            _unitPickerViewController.SpawnUnitClicked += HandleSpawnUnitClicked;

            for (int i = 0; i < 6; i++) {
                Vector2 startPosition = new Vector2(Random.Range(-3, 3), Random.Range(-3, 3));
                SpawnUnit(i, startPosition);
            }
        }

        public void Tick() {
            if (Input.GetKeyUp(KeyCode.Space)) {
                _unitPickerViewController.Show();
            }
        }

        private void HandleSpawnUnitClicked(IUnitData unitData) {
            _unitPickerViewController.Hide();

            int index = 0;
            foreach (var data in _unitDatas) {
                if (data == unitData) {
                    break;
                }
                
                index++;
            }

            SpawnUnit(index, Vector3.zero);
        }

        private void SpawnUnit(int index, Vector2 position) {
            _logger.Log(LoggedFeature.Units, "Spawning unit with index: {0}", index);
            UnitNetworkBehaviour unitNetworkBehaviour = _unitNetworkBehaviourPool.Spawn(index);
            
            // Temp code to set position. In the future, we will start the unit in the grid.
            unitNetworkBehaviour.transform.position =
                new Vector3(position.x, position.y, unitNetworkBehaviour.transform.position.z);
            
            NetworkServer.Spawn(unitNetworkBehaviour.gameObject);
        }
    }
}