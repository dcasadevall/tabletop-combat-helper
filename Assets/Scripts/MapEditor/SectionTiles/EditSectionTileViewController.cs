using System.Collections.Generic;
using Logging;
using Map;
using Map.Serialized;
using Math;
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
        private ILogger _logger;
        private int _selectedIndex = 0;

        [Inject]
        public void Construct(MapData mapData, ILogger logger) {
            _mapData = mapData;
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

            UniTask addSectionTask = _confirmButton.OnClickAsync();
            UniTask cancelTask = _cancelButton.OnClickAsync();
            await UniTask.WhenAny(addSectionTask, cancelTask);

            gameObject.SetActive(false);
        }

        private void HandleConfirmPressed() {
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