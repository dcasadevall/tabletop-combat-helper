using System;
using System.Collections.Generic;
using Logging;
using Units.Serialized;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ILogger = Logging.ILogger;

namespace Units.UI {
    public class UnitPickerViewController : MonoBehaviour, IUnitPickerViewController {
        public event Action<IUnitData> SpawnUnitClicked = delegate {};

        public Dropdown dropdown;
        public Button spawnButton;
        
        private int _selectedIndex = 0;
        
        private List<IUnitData> _unitDatas;
        private ILogger _logger;

        [Inject]
        public void Construct(List<IUnitData> unitDatas, ILogger logger) {
            _unitDatas = unitDatas;
            _logger = logger;
        }

        private void Start() {
            if (dropdown == null) {
                _logger.LogError(LoggedFeature.Units, "Dropdown not assigned.");
                return;
            }
            
            if (spawnButton == null) {
                _logger.LogError(LoggedFeature.Units, "Spawn button not assigned.");
                return;
            }
            
            spawnButton.onClick.AddListener(HandleOnSpawnButtonClicked);
            dropdown.onValueChanged.AddListener(HandleOnValueChanged);

            // Because this class is injected, make sure it is initialized as "hidden".
            Hide();
        }

        private void HandleOnSpawnButtonClicked() {
            if (_selectedIndex < 0 || _selectedIndex >= _unitDatas.Count) {
                _logger.LogError(LoggedFeature.Units, "Invalid selected index: {0}", _selectedIndex.ToString());
                return;
            }
            
            SpawnUnitClicked.Invoke(_unitDatas[_selectedIndex]);
        }

        public void Show() {
            if (dropdown == null) {
                _logger.LogError(LoggedFeature.Units, "Dropdown not assigned.");
                return;
            }
            
            if (spawnButton == null) {
                _logger.LogError(LoggedFeature.Units, "Spawn button not assigned.");
                return;
            }

            dropdown.ClearOptions();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            foreach (var unitData in _unitDatas) {
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
