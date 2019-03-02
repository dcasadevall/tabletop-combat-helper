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

        private void Start() {
            UnitPickerViewController.SpawnUnitClicked += HandleSpawnUnitClicked;
        }

        private void Update() {
            if (Input.GetKeyUp(KeyCode.S)) {
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
            instantiatedUnit.GetComponent<PlayerPrototype>().spriteRenderer.sprite = unitData.Sprite;
            NetworkServer.Spawn(instantiatedUnit);
        }
    }
}