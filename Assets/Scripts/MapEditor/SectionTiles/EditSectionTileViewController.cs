using System;
using System.Collections.Generic;
using Castle.Core.Internal;
using Logging;
using Map;
using Map.MapData;
using Map.MapSections.Commands;
using Math;
using ModestTree;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ILogger = Logging.ILogger;

namespace MapEditor.SectionTiles {
    public class EditSectionTileViewController : MonoBehaviour {
        [SerializeField]
        private Button _confirmButton;

        [SerializeField]
        private Button _cancelButton;

        [SerializeField]
        private Dropdown _sectionSelectDropdown;

        private IMapData _mapData;
        private IMutableMapSectionData _mutableMapSectionData;
        private ILogger _logger;
        private int _selectedIndex = 0;
        private IntVector2 _tileCoords;

        [Inject]
        public void Construct(IMapData mapData,
                              IMutableMapSectionData mutableMapSectionData, 
                              ILogger logger) {
            _mapData = mapData;
            _mutableMapSectionData = mutableMapSectionData;
            _logger = logger;
        }

        private void Awake() {
            _confirmButton.onClick.AddListener(HandleConfirmPressed);
            _sectionSelectDropdown.onValueChanged.AddListener(HandleSectionSelectValueChanged);
        }

        private void Start() {
            gameObject.SetActive(false);
            
            _sectionSelectDropdown.ClearOptions();
            var options = new List<Dropdown.OptionData>();
            foreach (var mapDataSection in _mapData.Sections) {
                options.Add(new Dropdown.OptionData(mapDataSection.SectionName));
            }
            _sectionSelectDropdown.AddOptions(options);
        }

        private void OnDestroy() {
            _confirmButton.onClick.RemoveListener(HandleConfirmPressed);
            _sectionSelectDropdown.onValueChanged.RemoveListener(HandleSectionSelectValueChanged);
        }

        public async UniTask Show(IntVector2 tileCoords) {
            gameObject.SetActive(true);

            // Show existing section connection if possible.
            _selectedIndex = 0;
            if (_mutableMapSectionData.TileMetadataMap.ContainsKey(tileCoords) &&
                _mutableMapSectionData.TileMetadataMap[tileCoords].SectionConnection != null) {
                _selectedIndex = (int) _mutableMapSectionData.TileMetadataMap[tileCoords].SectionConnection.Value;
            }
            _sectionSelectDropdown.value = _selectedIndex;
            
            // Wait on confirm / cancel
            _tileCoords = tileCoords;
            UniTask confirmTask = _confirmButton.OnClickAsync();
            UniTask cancelTask = _cancelButton.OnClickAsync();
            await UniTask.WhenAny(confirmTask, cancelTask);

            gameObject.SetActive(false);
        }

        private void HandleConfirmPressed() {
            if (_selectedIndex < 0 || _selectedIndex >= _mapData.Sections.Length) {
                _logger.LogError(LoggedFeature.MapEditor, "Invalid section index: {0}", _selectedIndex);
                return;
            }

            _mutableMapSectionData.SetSectionConnection(_tileCoords, (uint)_selectedIndex);
        }

        private void HandleSectionSelectValueChanged(int selectedIndex) {
            if (selectedIndex < 0 || selectedIndex >= _mapData.Sections.Length) {
                _logger.LogError(LoggedFeature.MapEditor, "Invalid section index: {0}", selectedIndex);
                return;
            }
            
            _selectedIndex = selectedIndex;
        }
    }
}