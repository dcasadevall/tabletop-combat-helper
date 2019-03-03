using System.Collections;
using Logging;
using Ninject;
using Ninject.Unity;
using Units.Serialized;
using Units.UI;
using UnityEngine;
using UnityEngine.Networking;
using ILogger = Logging.ILogger;

namespace Prototype {
    // TEMPORARY prototype class to spawn units.
    public class UnitSpawner : DIMono {
        public GameObject unitPrefab;
        
        [Inject]
        private IUnitPickerViewController UnitPickerViewController { get; set; }
        
        [Inject]
        private ILogger Logger { get; set; }
        
        [Inject]
        private IUnitData[] UnitDatas { get; set; }

        private void Start() {
            UnitPickerViewController.SpawnUnitClicked += HandleSpawnUnitClicked;

            for (int i = 0; i < 6; i++) {
                HandleSpawnUnitClicked(UnitDatas[i]);
            }
        }

        private void Update() {
            if (Input.GetKeyUp(KeyCode.Space)) {
                UnitPickerViewController.Show();
            }
        }

        private void HandleSpawnUnitClicked(IUnitData unitData) {
            UnitPickerViewController.Hide();
            if (unitPrefab == null) {
                Logger.Log(LoggedFeature.Units, "unitPrefab not assigned.");
                return;
            }
            
            GameObject instantiatedUnit = Instantiate(unitPrefab);
            int index = 0;
            foreach (var data in UnitDatas) {
                if (data == unitData) {
                    break;
                }
                
                index++;
            }

            Debug.Log("setting index: " + index);
            SetPlayerData(instantiatedUnit.GetComponent<PlayerPrototype>(), unitData, index);
            NetworkServer.Spawn(instantiatedUnit);
            SetPlayerData(instantiatedUnit.GetComponent<PlayerPrototype>(), unitData, index);
        }

        private void SetPlayerData(PlayerPrototype playerPrototype, IUnitData unitData, int index) {
            playerPrototype.spriteRenderer.sprite = unitData.Sprite;
            playerPrototype.avatarIconRenderer.sprite = unitData.AvatarSprite;
            playerPrototype.unitIndex = index;
        }
    }
}