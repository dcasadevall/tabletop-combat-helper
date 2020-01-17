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

        [Inject]
        public void Construct([Inject(Id = MapEditorInstaller.SECTION_TILE_EDITOR_ID)] IMapEditorTool sectionTileEditor) {
            _sectionTileEditor = sectionTileEditor;
        }

        private void Awake() {
            _sectionTileButton.onClick.AddListener(HandleSectionTileButtonPressed);
            _roomToolButton.onClick.AddListener(HandleRoomToolButtonPressed);
            _cancelButton.onClick.AddListener(HandleCancelButtonPressed);
        }

        private void OnDestroy() {
            _sectionTileButton.onClick.RemoveListener(HandleSectionTileButtonPressed);
            _roomToolButton.onClick.RemoveListener(HandleRoomToolButtonPressed);
            _cancelButton.onClick.RemoveListener(HandleCancelButtonPressed);
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
    }
}