using Grid;
using Grid.Positioning;
using Math;
using UnityEngine;
using Zenject;

namespace Prototype {
    /// <summary>
    /// Quick and dirty little behaviour to drag and drop 2d objects around the game.
    /// This will eventually be refactored
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class DragAndDropBehaviour : MonoBehaviour {
        // GHETTO remove
        public static bool isDragging = false;
        private Vector3 offset;
        
        private IGridPositionCalculator _gridPositionCalculator;
        private Camera _camera;

        [Inject]
        public void Construct(IGridPositionCalculator gridPositionCalculator, Camera camera) {
            _camera = camera;
            _gridPositionCalculator = gridPositionCalculator;
        }

        private void OnMouseDown() {
            offset = gameObject.transform.position - _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        }

        private void OnMouseUp() {
            isDragging = false;
        }

        private void OnMouseDrag() {
            isDragging = true;
            if (Input.GetKeyUp(KeyCode.R)) {
                transform.Rotate(Vector3.forward, 90);
            }
            
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Vector3 curPosition = _camera.ScreenToWorldPoint(curScreenPoint) + offset;
            IntVector2? gridCoordinates = _gridPositionCalculator.GetTileContainingWorldPosition(curPosition);
            
            if (gridCoordinates == null) {
                return;
            }
            
            Vector2 positionInGrid =
                _gridPositionCalculator.GetTileCenterWorldPosition(gridCoordinates.Value);
            transform.position = new Vector3(positionInGrid.x, positionInGrid.y, transform.position.z);
        }
    }
}
