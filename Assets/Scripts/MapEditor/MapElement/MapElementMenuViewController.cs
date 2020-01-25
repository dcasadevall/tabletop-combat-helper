using Async;
using Grid.Positioning;
using Math;
using UI.RadialMenu;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace MapEditor.MapElement {
    public class MapElementMenuViewController : MonoBehaviour, IMapElementMenuViewController {
        private Camera _camera;
        private IRadialMenu _radialMenu;
        private IGridPositionCalculator _gridPositionCalculator;

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

            var result = await _removeButton.OnClickAsObservable()
                                            .First()
                                            .ToButtonCancellableTask(_cancelButton);
            if (!result.isCanceled) {
                mapElement.Remove();
            }

            _radialMenu.Hide();
        }
    }
}