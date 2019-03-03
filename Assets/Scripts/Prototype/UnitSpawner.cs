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
    public class UnitSpawner : MonoBehaviour {
        private IUnitPickerViewController _unitPickerViewController;
        private ILogger _logger;
        private List<IUnitData> _unitDatas;
        private UnitNetworkBehaviour.Pool _unitNetworkBehaviourPool;

        [Inject]
        public void Construct(IUnitPickerViewController unitPickerVc, ILogger logger, List<IUnitData> unitDatas,
                              UnitNetworkBehaviour.Pool unitNetworkBehaviourPool) {
            _logger = logger;
            _unitPickerViewController = unitPickerVc;
            _unitDatas = unitDatas;
            _unitNetworkBehaviourPool = unitNetworkBehaviourPool;
        }

        private void Start() {
            _unitPickerViewController.SpawnUnitClicked += HandleSpawnUnitClicked;

            for (int i = 0; i < 6; i++) {
                HandleSpawnUnitClicked(_unitDatas[i]);
            }
        }

        private void Update() {
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

            _logger.Log(LoggedFeature.Units, "Spawning unit with index: {0}", index);
            UnitNetworkBehaviour unitNetworkBehaviour = _unitNetworkBehaviourPool.Spawn(index);
            NetworkServer.Spawn(unitNetworkBehaviour.gameObject);
        }
    }
}