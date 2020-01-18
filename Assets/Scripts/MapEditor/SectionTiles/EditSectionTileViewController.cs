using System.Collections.Generic;
using Map;
using Math;
using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace MapEditor.SectionTiles {
    public class EditSectionTileViewController : MonoBehaviour {
        [SerializeField]
        private Button _addSectionTileButton;

        [SerializeField]
        private Button _cancelButton;

        [SerializeField]
        private Dropdown _sectionSelectDropdown;

        private IMapData _mapData;

        [Inject]
        public void Construct(IMapData mapData) {
            _mapData = mapData;
        }

        private void Awake() {
            _addSectionTileButton.onClick.AddListener(HandleAddSectionTilePressed);
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
            _addSectionTileButton.onClick.RemoveListener(HandleAddSectionTilePressed);
            _sectionSelectDropdown.onValueChanged.RemoveListener(HandleSectionSelectValueChanged);
        }

        public async UniTask Show(IntVector2 tileCoords) {
            gameObject.SetActive(true);

            UniTask addSectionTask = _addSectionTileButton.OnClickAsync();
            UniTask cancelTask = _cancelButton.OnClickAsync();
            await UniTask.WhenAny(addSectionTask, cancelTask);

            gameObject.SetActive(false);
        }

        private void HandleAddSectionTilePressed() { }

        private void HandleSectionSelectValueChanged(int selectedIndex) { }
    }
}