using System;
using System.Collections.Generic;
using Logging;
using Ninject;
using Ninject.Unity;
using Units.Serialized;
using UnityEngine.UI;
using ILogger = Logging.ILogger;

namespace Units.UI {
    public class UnitPickerViewController : DIMono, IUnitPickerViewController {
        public event Action<IUnitData> SpawnUnitClicked = delegate {};

        public Dropdown dropdown;
        public Button spawnButton;
        
        private int _selectedIndex = 0;
        
        [Inject]
        private ILogger Logger { get; set; }
        
        [Inject] 
        private IUnitData[] unitDatas { get; set; }

        private void Start() {
            if (dropdown == null) {
                Logger.LogError(LoggedFeature.Units, "Dropdown not assigned.");
                return;
            }
            
            if (spawnButton == null) {
                Logger.LogError(LoggedFeature.Units, "Spawn button not assigned.");
                return;
            }
            
            spawnButton.onClick.AddListener(HandleOnSpawnButtonClicked);
            dropdown.onValueChanged.AddListener(HandleOnValueChanged);

            // Because this class is injected, make sure it is initialized as "hidden".
            Hide();
        }

        private void HandleOnSpawnButtonClicked() {
            if (_selectedIndex < 0 || _selectedIndex >= unitDatas.Length) {
                Logger.LogError(LoggedFeature.Units, "Invalid selected index: {0}", _selectedIndex.ToString());
                return;
            }
            
            SpawnUnitClicked.Invoke(unitDatas[_selectedIndex]);
        }

        public void Show() {
            if (dropdown == null) {
                Logger.LogError(LoggedFeature.Units, "Dropdown not assigned.");
                return;
            }
            
            if (spawnButton == null) {
                Logger.LogError(LoggedFeature.Units, "Spawn button not assigned.");
                return;
            }

            dropdown.ClearOptions();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            foreach (var unitData in unitDatas) {
                options.Add(new Dropdown.OptionData(unitData.Name, unitData.Sprite));
            }
            dropdown.AddOptions(options);
            
            gameObject.SetActive(true);
        }

        public void Hide() {
            gameObject.SetActive(false);
        }

        private void HandleOnValueChanged(int selectedIndex) {
            _selectedIndex = selectedIndex;
        }
    }
}
