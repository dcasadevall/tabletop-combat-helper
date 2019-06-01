using CommandSystem;
using Map.Commands;
using Map.MapSections.Commands;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Map.MapSections.UI {
    public class MapSectionSelectionViewController : MonoBehaviour {
        [SerializeField]
        private Button _nextSectionButton;

        [SerializeField]
        private Button _previousSectionButton;

        private IMapSectionContext _mapSectionContext;
        private IMapData _mapData;
        private ICommandQueue _commandQueue;
        private LoadMapCommandData _loadMapCommandData;

        [Inject]
        public void Construct(IMapSectionContext mapSectionContext, IMapData mapData,
                              LoadMapCommandData loadMapCommandData, ICommandQueue commandQueue) {
            _nextSectionButton.onClick.AddListener(HandleNextSectionButtonCLicked);
            _previousSectionButton.onClick.AddListener(HandlePreviousSectionButtonCLicked);

            _mapSectionContext = mapSectionContext;
            _loadMapCommandData = loadMapCommandData;
            _mapData = mapData;
            _commandQueue = commandQueue;
        }

        private void Update() {
            _previousSectionButton.interactable = _mapSectionContext.CurrentSectionIndex > 0;
            _nextSectionButton.interactable = _mapSectionContext.CurrentSectionIndex < _mapData.Sections.Length - 1;
        }

        private void HandleNextSectionButtonCLicked() {
            LoadMapSectionCommandData commandData =
                new LoadMapSectionCommandData(_mapSectionContext.CurrentSectionIndex + 1, _loadMapCommandData);
            _commandQueue.Enqueue<LoadMapSectionCommand, LoadMapSectionCommandData>(commandData, CommandSource.Game);
        }

        private void HandlePreviousSectionButtonCLicked() {
            LoadMapSectionCommandData commandData =
                new LoadMapSectionCommandData(_mapSectionContext.CurrentSectionIndex - 1, _loadMapCommandData);
            _commandQueue.Enqueue<LoadMapSectionCommand, LoadMapSectionCommandData>(commandData, CommandSource.Game);
        }
    }
}