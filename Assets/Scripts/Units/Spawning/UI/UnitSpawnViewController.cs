using System;
using System.Collections.Generic;
using CommandSystem;
using Grid.Positioning;
using Logging;
using Math;
using UniRx.Async;
using Units.Serialized;
using Units.Spawning.Commands;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;
using ILogger = Logging.ILogger;

namespace Units.Spawning.UI {
    public class UnitSpawnViewController : MonoBehaviour, IUnitSpawnViewController {
#pragma warning disable 649
        [SerializeField]
        private Dropdown _dropdown;

        [SerializeField]
        private Dropdown _unitAmountDropdown;

        [SerializeField]
        private Button _spawnButton;

        [SerializeField]
        private Button _cancelButton;
#pragma warning restore 649

        private IUnitData[] _unitDatas;
        private IUnitDataIndexResolver _unitDataIndexResolver;
        private IRandomGridPositionProvider _randomGridPositionProvider;
        private ICommandQueue _commandQueue;
        private IFactory<IUnitData, UnitCommandData> _unitCommandDataFactory;
        private ILogger _logger;

        [Inject]
        public void Construct(IUnitSpawnSettings unitSpawnSettings, 
                              IUnitDataIndexResolver unitDataIndexResolver,
                              IRandomGridPositionProvider randomGridPositionProvider, 
                              ICommandQueue commandQueue,
                              IFactory<IUnitData, UnitCommandData> unitCommandDataFactory,
                              ILogger logger) {
            _unitDatas = unitSpawnSettings.GetUnits(UnitType.NonPlayer);
            _unitDataIndexResolver = unitDataIndexResolver;
            _randomGridPositionProvider = randomGridPositionProvider;
            _commandQueue = commandQueue;
            _unitCommandDataFactory = unitCommandDataFactory;
            _logger = logger;

            Preconditions.CheckNotNull(_dropdown, _spawnButton, _cancelButton, _unitAmountDropdown);
        }

        private void Awake() {
            // Start hidden by default.
            gameObject.SetActive(false);
            
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
        }

        private void Update() {
            for (int i = 1; i < 10; ++i) {
                if (Input.GetKeyDown("" + i)) {
                    _unitAmountDropdown.value = i - 1;
                }
            }
        }

        public async UniTask Show(IntVector2 tileCoords) {
            // Show the UI, subscribe to events
            gameObject.SetActive(true);

            var spawnButtonTask = _spawnButton.OnClickAsync();
            var cancelButtonTask = _cancelButton.OnClickAsync();
            var eventReceived = await UniTask.WhenAny(spawnButtonTask, cancelButtonTask);
            if (eventReceived == 0) {
                HandleOnSpawnButtonClicked(tileCoords);
            }

            gameObject.SetActive(false);
        }

        private void HandleOnSpawnButtonClicked(IntVector2 tileCoords) {
            var selectedIndex = (uint) _dropdown.value;
            int numUnits = _unitAmountDropdown.value + 1;
            IUnitData unitData = _unitDataIndexResolver.ResolveUnitData(UnitType.NonPlayer, selectedIndex);
            if (unitData == null) {
                _logger.LogError(LoggedFeature.Units, "Invalid unit index: {0}", selectedIndex);
                return;
            }

            IntVector2[] tilePositions = _randomGridPositionProvider.GetRandomUniquePositions(tileCoords, 1, numUnits);
            foreach (var tilePosition in tilePositions) {
                SpawnUnit(unitData, tilePosition);
            }
        }

        private void SpawnUnit(IUnitData unitData, IntVector2 tileCoords) {
            UnitCommandData unitCommandData = _unitCommandDataFactory.Create(unitData);
            SpawnUnitData spawnUnitData = new SpawnUnitData(unitCommandData, tileCoords, isInitialSpawn: false);
            _commandQueue.Enqueue<SpawnUnitCommand, SpawnUnitData>(spawnUnitData, CommandSource.Game);
        }
    }
}