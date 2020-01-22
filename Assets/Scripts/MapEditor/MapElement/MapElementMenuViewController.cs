using Grid.Positioning;
using Math;
using UI.RadialMenu;
using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace MapEditor.MapElement {
    public class MapElementMenuViewController : MonoBehaviour, IMapElementMenuViewController {
        private Camera _camera;
        private IRadialMenu _radialMenu;
        private IGridPositionCalculator _gridPositionCalculator;
        private IMapElement _mapElement;

        [SerializeField]
        private Button _removeButton;
        
        [SerializeField]
        private Button _cancelButton;

        [Inject]
        public void Construct(Camera camera, IGridPositionCalculator gridPositionCalculator) {
            _camera = camera;
            _gridPositionCalculator = gridPositionCalculator;
            _radialMenu = GetComponent<IRadialMenu>();
        }

        public async void Show(IntVector2 tileCoords, IMapElement mapElement) {
            Vector3 screenPosition =
                _camera.WorldToScreenPoint(_gridPositionCalculator.GetTileCenterWorldPosition(tileCoords));
            _radialMenu.Show(screenPosition);
            _removeButton.onClick.AddListener(HandleRemoveButtonClicked);
            _mapElement = mapElement;
            
            var removeTask = _removeButton.OnClickAsync();
            var cancelTask = _cancelButton.OnClickAsync();
            await UniTask.WhenAny(removeTask, cancelTask);

            _removeButton.onClick.RemoveListener(HandleRemoveButtonClicked);
            _radialMenu.Hide();
        }

        private void HandleRemoveButtonClicked() {
            _mapElement.Remove();
        }
    }
}