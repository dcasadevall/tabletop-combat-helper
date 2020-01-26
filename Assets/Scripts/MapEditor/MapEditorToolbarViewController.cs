using System;
using InputSystem;
using Logging;
using Map.MapData.Store;
using Map.MapSections.Commands;
using UniRx;
using UniRx.Async;
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
        private IInputEvents _inputEvents;
        private ILogger _logger;

        private IDisposable _cancelObserver;

        [Inject]
        public void Construct(MapStoreId mapStoreId,
                              [Inject(Id = MapEditorInstaller.SECTION_TILE_EDITOR_ID)]
                              IMapEditorTool sectionTileEditor,
                              [Inject(Id = MapEditorInstaller.UNIT_TILE_EDITOR_ID)]
                              IMapEditorTool unitTileEditor,
                              IMapDataStore mapDataStore,
                              IInputLock inputLock,
                              IInputEvents inputEvents,
                              ILogger logger) {
            _mapStoreId = mapStoreId;
            _sectionTileEditor = sectionTileEditor;
            _unitTileEditor = unitTileEditor;
            _mapDataStore = mapDataStore;
            _inputLock = inputLock;
            _inputEvents = inputEvents;
            _logger = logger;
        }

        private void Awake() {
            _addUnitButton.onClick.AddListener(HandleAddUnitButtonPressed);
            _sectionTileButton.onClick.AddListener(HandleSectionTileButtonPressed);
            _roomToolButton.onClick.AddListener(HandleRoomToolButtonPressed);
            _saveButton.onClick.AddListener(HandleSaveButtonPressed);

            _inputLock.InputLockAcquired += HandleInputLockAcquired;
            _inputLock.InputLockReleased += HandleInputLockReleased;
        }

        private void OnDestroy() {
            _addUnitButton.onClick.RemoveListener(HandleAddUnitButtonPressed);
            _sectionTileButton.onClick.RemoveListener(HandleSectionTileButtonPressed);
            _roomToolButton.onClick.RemoveListener(HandleRoomToolButtonPressed);
            _saveButton.onClick.RemoveListener(HandleSaveButtonPressed);

            _inputLock.InputLockAcquired -= HandleInputLockAcquired;
            _inputLock.InputLockReleased -= HandleInputLockReleased;
        }

        private void HandleInputLockAcquired() {
            gameObject.SetActive(false);
        }

        private void HandleInputLockReleased() {
            gameObject.SetActive(true);
        }

        private async void HandleAddUnitButtonPressed() {
            using (_inputLock.Lock()) {
                gameObject.SetActive(true);
                _toolbarContainer.SetActive(false);
                _cancelContainer.SetActive(true);

                _unitTileEditor.StartEditing();
                await UniTask.WhenAny(_cancelButton.OnClickAsync(),
                                      _inputEvents.RightMouseClick.First().ToUniTask());
                _unitTileEditor.StopEditing();

                _toolbarContainer.SetActive(true);
                _cancelContainer.SetActive(false);
            }
        }

        private async void HandleSectionTileButtonPressed() {
            using (_inputLock.Lock()) {
                gameObject.SetActive(true);
                _toolbarContainer.SetActive(false);
                _cancelContainer.SetActive(true);

                _sectionTileEditor.StartEditing();
                // Either cancel button, right click, or section load (because we have 1 toolbar per section) will quit.
                await UniTask.WhenAny(_cancelButton.OnClickAsync(),
                                      _inputEvents.RightMouseClick.First().ToUniTask());
                _sectionTileEditor.StopEditing();

                _toolbarContainer.SetActive(true);
                _cancelContainer.SetActive(false);
            }
        }

        private async void HandleRoomToolButtonPressed() {
            using (_inputLock.Lock()) {
                gameObject.SetActive(true);
                _toolbarContainer.SetActive(false);
                _cancelContainer.SetActive(true);

                await UniTask.WhenAny(_cancelButton.OnClickAsync(),
                                      _inputEvents.RightMouseClick.First().ToUniTask());

                _toolbarContainer.SetActive(true);
                _cancelContainer.SetActive(false);
            }
        }

        private void HandleSaveButtonPressed() {
            if (_mapDataStore.Commit(_mapStoreId)) {
                _logger.Log(LoggedFeature.MapEditor, "Successfully saved map data.");
            } else {
                _logger.LogError(LoggedFeature.MapEditor, "Error saving map data.");
            }
        }
    }
}