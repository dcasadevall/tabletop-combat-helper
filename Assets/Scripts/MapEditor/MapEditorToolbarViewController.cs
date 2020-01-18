using InputSystem;
using Map.MapSections.Commands;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace MapEditor {
    public class MapEditorToolbarViewController : MonoBehaviour {
        [SerializeField]
        private Button _sectionTileButton;

        [SerializeField]
        private Button _roomToolButton;

        [SerializeField]
        private Button _cancelButton;

        [SerializeField]
        private GameObject _toolbarContainer;

        [SerializeField]
        private GameObject _cancelContainer;

        private IMapEditorTool _sectionTileEditor;
        private IInputLock _inputLock;
        private SignalBus _signalBus;

        [Inject]
        public void Construct([Inject(Id = MapEditorInstaller.SECTION_TILE_EDITOR_ID)]
                              SignalBus signalBus,
                              IMapEditorTool sectionTileEditor, 
                              IInputLock inputLock) {
            _signalBus = signalBus;
            _sectionTileEditor = sectionTileEditor;
            _inputLock = inputLock;
        }

        private void Awake() {
            _signalBus.Subscribe<MapSectionWillLoadSignal>(HandleMapSectionWillLoad);
            _sectionTileButton.onClick.AddListener(HandleSectionTileButtonPressed);
            _roomToolButton.onClick.AddListener(HandleRoomToolButtonPressed);
            _cancelButton.onClick.AddListener(HandleCancelButtonPressed);
            _inputLock.InputLockAcquired += HandleInputLockAcquired;
            _inputLock.InputLockReleased += HandleInputLockReleased;
        }

        private void OnDestroy() {
            _sectionTileButton.onClick.RemoveListener(HandleSectionTileButtonPressed);
            _roomToolButton.onClick.RemoveListener(HandleRoomToolButtonPressed);
            _cancelButton.onClick.RemoveListener(HandleCancelButtonPressed);
            _inputLock.InputLockAcquired -= HandleInputLockAcquired;
            _inputLock.InputLockReleased -= HandleInputLockReleased;
        }
        
        private void HandleInputLockAcquired() {
            gameObject.SetActive(false);
        }

        private void HandleInputLockReleased() {
            gameObject.SetActive(true);
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

        private void HandleCancelButtonPressed() {
            _sectionTileEditor.StopEditing();

            _toolbarContainer.SetActive(true);
            _cancelContainer.SetActive(false);
        }
        
        // On section load, exit all editing. Because we have 1 toolbar per section.
        private void HandleMapSectionWillLoad(MapSectionWillLoadSignal signal) {
            HandleCancelButtonPressed();
        }
    }
}