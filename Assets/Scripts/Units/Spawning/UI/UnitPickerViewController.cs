﻿using System;
using System.Collections.Generic;
using Logging;
using Units.Serialized;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ILogger = Logging.ILogger;

namespace Units.Spawning.UI {
    public class UnitPickerViewController : MonoBehaviour, IUnitPickerViewController, IInitializable, IDisposable {
        public event System.Action ViewControllerDismissed;
        public event SpawnUnitClickedDelegate SpawnUnitClicked;

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
        
        private uint _selectedIndex = 0;

        private IUnitData[] _unitDatas;
        private IUnitDataIndexResolver _unitDataIndexResolver;
        private ILogger _logger;

        [Inject]
        public void Construct(IUnitSpawnSettings unitSpawnSettings, IUnitDataIndexResolver unitDataIndexResolver, ILogger logger) {
            _unitDatas = unitSpawnSettings.GetUnits(UnitType.NonPlayer);
            _unitDataIndexResolver = unitDataIndexResolver;
            _logger = logger;
        }

        public void Initialize() {
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
            
            // Prefab is instantiated on scene load, so make sure we start hidden.
            Hide();
        }

        public void Dispose() {
            _spawnButton.onClick.RemoveListener(HandleOnSpawnButtonClicked);
            _dropdown.onValueChanged.RemoveListener(HandleOnValueChanged);
        }

        private void HandleOnSpawnButtonClicked() {
            IUnitData unitData = _unitDataIndexResolver.ResolveUnitData(UnitType.NonPlayer, _selectedIndex);
            if (unitData == null) {
                _logger.LogError(LoggedFeature.Units, "Invalid unit index: {0}", _selectedIndex);
                return;
            }
            
            Hide();
            ViewControllerDismissed?.Invoke();
            SpawnUnitClicked?.Invoke(unitData, _unitAmountDropdown.value + 1);
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
            // _uiAnchor.position = Input.mousePosition;
            
            // Show the UI
            gameObject.SetActive(true);
        }

        private void Hide() {
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
            _selectedIndex = (uint)selectedIndex;
        }
    }
}