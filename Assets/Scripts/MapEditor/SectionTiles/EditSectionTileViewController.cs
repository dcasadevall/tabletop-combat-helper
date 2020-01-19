using System.Collections.Generic;
using Castle.Core.Internal;
using Logging;
using Map;
using Map.Serialized;
using Math;
using ModestTree;
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

        private MapData _mapData;
        private IMapSectionData _mapSectionData;
        private ILogger _logger;
        private int _selectedIndex = 0;
        private IntVector2 _tileCoords;

        [Inject]
        public void Construct(IMapSectionData mapSectionData, MapData mapData, ILogger logger) {
            _mapData = mapData;
            _mapSectionData = mapSectionData;
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
            if (_mapSectionData.TileMetadataMap.ContainsKey(tileCoords) &&
                _mapSectionData.TileMetadataMap[tileCoords].SectionConnection != null) {
                _selectedIndex = (int) _mapSectionData.TileMetadataMap[tileCoords].SectionConnection.Value;
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

            MapSectionData mapSectionData = _mapData.sections[_selectedIndex];
            int index = mapSectionData.tileMetadataPairs.FindIndex(x => IntVector2.Of(x.tileCoords) == _tileCoords);

            if (index == -1) {
                mapSectionData.tileMetadataPairs.Add(new TileMetadataPair(new Vector2(_tileCoords.x, _tileCoords.y)));
                index = mapSectionData.tileMetadataPairs.Count - 1;
            }

            TileMetadata tileMetadata = mapSectionData.tileMetadataPairs[index].tileMetadata;
            tileMetadata.sectionConnection = _selectedIndex;
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