using System;
using System.Collections.Generic;
using Grid.Positioning;
using Logging;
using Math;
using Units.Serialized;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ILogger = Logging.ILogger;

namespace Units.UI {
    public class UnitPickerViewController : MonoBehaviour, IUnitPickerViewController {
        public event Action<IUnitData, int> SpawnUnitClicked = delegate {};

#pragma warning disable 649
        [SerializeField]
        private Dropdown _dropdown;
        
        [SerializeField]
        private Dropdown _unitAmountDropdown;
        
        [SerializeField]
        private Button _spawnButton;

        [SerializeField]
        private Transform _uiAnchor;
#pragma warning restore 649
        
        private int _selectedIndex = 0;

        private List<IUnitData> _unitDatas;
        private ILogger _logger;

        [Inject]
        public void Construct(List<IUnitData> unitDatas, ILogger logger) {
            _unitDatas = unitDatas;
            _logger = logger;
        }

        private void Start() {
            if (_dropdown == null) {
                _logger.LogError(LoggedFeature.Units, "Dropdown not assigned.");
                return;
            }
            
            if (_spawnButton == null) {
                _logger.LogError(LoggedFeature.Units, "Spawn button not assigned.");
                return;
            }
            
            _spawnButton.onClick.AddListener(HandleOnSpawnButtonClicked);
            _dropdown.onValueChanged.AddListener(HandleOnValueChanged);

            // Because this class is injected, make sure it is initialized as "hidden".
            Hide();
        }

        private void HandleOnSpawnButtonClicked() {
            if (_selectedIndex < 0 || _selectedIndex >= _unitDatas.Count) {
                _logger.LogError(LoggedFeature.Units, "Invalid selected index: {0}", _selectedIndex.ToString());
                return;
            }
            
            SpawnUnitClicked.Invoke(_unitDatas[_selectedIndex], _unitAmountDropdown.value + 1);
        }

        public void Show() {
            if (_dropdown == null) {
                _logger.LogError(LoggedFeature.Units, "Dropdown not assigned.");
                return;
            }
            
            if (_spawnButton == null) {
                _logger.LogError(LoggedFeature.Units, "Spawn button not assigned.");
                return;
            }

            // Initialize unit dropdown
            _dropdown.ClearOptions();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            foreach (var unitData in _unitDatas) {
                options.Add(new Dropdown.OptionData(unitData.Name, unitData.Sprite));
            }
            _dropdown.AddOptions(options);
            
            // initialize unit count dropdown
            _unitAmountDropdown.ClearOptions();
            options = new List<Dropdown.OptionData>();
            for (int i = 1; i < 10; i++) {
                options.Add(new Dropdown.OptionData(i.ToString()));
            }
            _unitAmountDropdown.AddOptions(options);

            // Set UI anchored to wherever the mouse is.
            _uiAnchor.position = Input.mousePosition;
            
            // Show the UI
            gameObject.SetActive(true);
        }

        public void Hide() {
            gameObject.SetActive(false);
        }

        private void Update() {
            for (int i = 1; i < 10; ++i) {
                if (Input.GetKeyDown("" + i)) {
                    _unitAmountDropdown.value = i - 1;
                }
            }
        }

        private void HandleOnValueChanged(int selectedIndex) {
            _selectedIndex = selectedIndex;
        }
    }
}
