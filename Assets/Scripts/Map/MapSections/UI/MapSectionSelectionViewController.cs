using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Map.MapSections.UI {
    public class MapSectionSelectionViewController : MonoBehaviour {
        [SerializeField]
        private Button _nextSectionButton;

        [SerializeField]
        private Button _previousSectionButton;

        private MapSectionContext _mapSectionContext;
        private IMapData _mapData;

        [Inject]
        public void Construct(MapSectionContext mapSectionContext, IMapData mapData) {
            _nextSectionButton.onClick.AddListener(HandleNextSectionButtonCLicked);
            _previousSectionButton.onClick.AddListener(HandlePreviousSectionButtonCLicked);

            _mapSectionContext = mapSectionContext;
            _mapData = mapData;
            
            UpdateSectionButtons();
        }

        private void UpdateSectionButtons() {
            _previousSectionButton.interactable = _mapSectionContext.CurrentSectionIndex > 0;
            _nextSectionButton.interactable = _mapSectionContext.CurrentSectionIndex < _mapData.Sections.Length - 1;
        }

        private void HandleNextSectionButtonCLicked() {
            _mapSectionContext.CurrentSectionIndex =
                System.Math.Min(_mapData.Sections.Length - 1, _mapSectionContext.CurrentSectionIndex + 1);
            
            UpdateSectionButtons();
        }

        private void HandlePreviousSectionButtonCLicked() {
            _mapSectionContext.CurrentSectionIndex =
                System.Math.Max(0, _mapSectionContext.CurrentSectionIndex - 1);
            
            UpdateSectionButtons();
        }
    }
}