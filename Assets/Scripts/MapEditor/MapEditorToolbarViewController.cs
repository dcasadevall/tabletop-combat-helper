using InputSystem;
using Logging;
using Map.MapData.Store;
using Map.MapSections.Commands;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ILogger = Logging.ILogger;

namespace MapEditor {
    public class MapEditorToolbarViewController : MonoBehaviour {
        [SerializeField]
        private Button _sectionTileButton;

        [SerializeField]
        private Button _roomToolButton;
        
        [SerializeField]
        private Button _addUnitButton;

        [SerializeField]
        private Button _saveButton;

        [SerializeField]
        private Button _cancelButton;

        [SerializeField]
        private GameObject _toolbarContainer;

        [SerializeField]
        private GameObject _cancelContainer;

        private MapStoreId _mapStoreId;
        private IMapEditorTool _sectionTileEditor;
        private IMapEditorTool _unitTileEditor;
        private IMapDataStore _mapDataStore;
        private IInputLock _inputLock;
        private ILogger _logger;

        [Inject]
        public void Construct(MapStoreId mapStoreId,
                              [Inject(Id = MapEditorInstaller.SECTION_TILE_EDITOR_ID)]
                              IMapEditorTool sectionTileEditor,
                              [Inject(Id = MapEditorInstaller.UNIT_TILE_EDITOR_ID)]
                              IMapEditorTool unitTileEditor,
                              IMapDataStore mapDataStore,
                              IInputLock inputLock,
                              ILogger logger) {
            _mapStoreId = mapStoreId;
            _sectionTileEditor = sectionTileEditor;
            _unitTileEditor = unitTileEditor;
            _mapDataStore = mapDataStore;
            _inputLock = inputLock;
            _logger = logger;
        }

        private void Awake() {
            _addUnitButton.onClick.AddListener(HandleAddUnitButtonPressed);
            _sectionTileButton.onClick.AddListener(HandleSectionTileButtonPressed);
            _roomToolButton.onClick.AddListener(HandleRoomToolButtonPressed);
            _saveButton.onClick.AddListener(HandleSaveButtonPressed);
            _cancelButton.onClick.AddListener(HandleCancelButtonPressed);

            LoadMapSectionCommand.MapSectionWillLoad += HandleMapSectionWillLoad;
            _inputLock.InputLockAcquired += HandleInputLockAcquired;
            _inputLock.InputLockReleased += HandleInputLockReleased;
        }

        private void OnDestroy() {
            _addUnitButton.onClick.RemoveListener(HandleAddUnitButtonPressed);
            _sectionTileButton.onClick.RemoveListener(HandleSectionTileButtonPressed);
            _roomToolButton.onClick.RemoveListener(HandleRoomToolButtonPressed);
            _saveButton.onClick.RemoveListener(HandleSaveButtonPressed);
            _cancelButton.onClick.RemoveListener(HandleCancelButtonPressed);

            LoadMapSectionCommand.MapSectionWillLoad -= HandleMapSectionWillLoad;
            _inputLock.InputLockAcquired -= HandleInputLockAcquired;
            _inputLock.InputLockReleased -= HandleInputLockReleased;
        }

        private void HandleInputLockAcquired() {
            gameObject.SetActive(false);
        }

        private void HandleInputLockReleased() {
            gameObject.SetActive(true);
        }

        private void HandleAddUnitButtonPressed() {
            _unitTileEditor.StartEditing();
            
            _toolbarContainer.SetActive(false);
            _cancelContainer.SetActive(true);
        }

        private void HandleSectionTileButtonPressed() {
            _sectionTileEditor.StartEditing();

            _toolbarContainer.SetActive(false);
            _cancelContainer.SetActive(true);
        }

        private void HandleRoomToolButtonPressed() {
            _toolbarContainer.SetActive(false);
            _cancelContainer.SetActive(true);
        }

        private void HandleSaveButtonPressed() {
            if (_mapDataStore.Commit(_mapStoreId)) {
                _logger.Log(LoggedFeature.MapEditor, "Successfully saved map data.");
            } else {
                _logger.LogError(LoggedFeature.MapEditor, "Error saving map data.");
            }
        }

        private void HandleCancelButtonPressed() {
            _sectionTileEditor.StopEditing();
            _unitTileEditor.StopEditing();

            _toolbarContainer.SetActive(true);
            _cancelContainer.SetActive(false);
        }

        // On section load, exit all editing. Because we have 1 toolbar per section.
        private void HandleMapSectionWillLoad() {
            HandleCancelButtonPressed();
        }
    }
}