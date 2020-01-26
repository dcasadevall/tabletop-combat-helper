using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Logging;
using Map.MapData;
using Math;
using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ILogger = Logging.ILogger;

namespace MapEditor.SectionTiles {
    /// <summary>
    /// ViewController used to edit / add a section tile.
    /// </summary>
    public class EditSectionTileViewController : MonoBehaviour {
        [SerializeField]
        private Button _confirmButton;

        [SerializeField]
        private Button _cancelButton;

        [SerializeField]
        private Dropdown _sectionSelectDropdown;

        private IMapData _mapData;
        private IMutableMapSectionData _mapSectionData;
        private ILogger _logger;
        
        private Dictionary<uint, int> _dropdownIndexMap = new Dictionary<uint, int>();
        private int _selectedIndex = 0;
        private IntVector2 _tileCoords;

        [Inject]
        public void Construct(IMapData mapData,
                              IMutableMapSectionData mutableMapSectionData, 
                              ILogger logger) {
            _mapData = mapData;
            _mapSectionData = mutableMapSectionData;
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

            int i = 0;
            foreach (var sectionConnection in _mapData.Sections) {
                // Do not include this own section
                if (sectionConnection.SectionIndex == _mapSectionData.SectionIndex) {
                    continue;
                }

                options.Add(new Dropdown.OptionData(sectionConnection.SectionName));
                _dropdownIndexMap[sectionConnection.SectionIndex] = i;
                i++;
            }
            _sectionSelectDropdown.AddOptions(options);
        }

        private void OnDestroy() {
            _confirmButton.onClick.RemoveListener(HandleConfirmPressed);
            _sectionSelectDropdown.onValueChanged.RemoveListener(HandleSectionSelectValueChanged);
        }

        public async UniTask Show(IntVector2 tileCoords, CancellationToken cancellationToken) {
            gameObject.SetActive(true);

            // Show existing section connection if possible.
            _selectedIndex = 0;
            if (_mapSectionData.TileMetadataMap.ContainsKey(tileCoords) &&
                _mapSectionData.TileMetadataMap[tileCoords].SectionConnection != null) {
                var connectionIndex = _mapSectionData.TileMetadataMap[tileCoords].SectionConnection.Value;
                _selectedIndex = _dropdownIndexMap[connectionIndex];
            }
            _sectionSelectDropdown.value = _selectedIndex;
            
            // Wait on confirm / cancel / cancel token
            _tileCoords = tileCoords;
            UniTask confirmTask = _confirmButton.OnClickAsync();
            UniTask cancelTask = _cancelButton.OnClickAsync();
            (UniTask cancelTokenTask, CancellationTokenRegistration tokenRegistration) = cancellationToken.ToUniTask();
            await UniTask.WhenAny(confirmTask, cancelTask, cancelTokenTask);

            gameObject.SetActive(false);
            tokenRegistration.Dispose();
        }

        private void HandleConfirmPressed() {
            if (_selectedIndex < 0 || _selectedIndex >= _mapData.Sections.Length) {
                _logger.LogError(LoggedFeature.MapEditor, "Invalid section index: {0}", _selectedIndex);
                return;
            }

            uint sectionIndex = _dropdownIndexMap.FirstOrDefault(x => x.Value == _selectedIndex).Key;
            _mapSectionData.SetSectionConnection(_tileCoords, sectionIndex);
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