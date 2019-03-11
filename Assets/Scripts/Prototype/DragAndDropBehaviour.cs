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
        
        private IGrid _grid;
        private IGridPositionCalculator _gridPositionCalculator;

        [Inject]
        public void Construct(IGrid grid, IGridPositionCalculator gridPositionCalculator) {
            _grid = grid;
            _gridPositionCalculator = gridPositionCalculator;
        }

        private void OnMouseDown() {
            offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
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
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
            IntVector2? gridCoordinates = _gridPositionCalculator.GetTileContainingWorldPosition(_grid, curPosition);
            
            if (gridCoordinates == null) {
                return;
            }
            
            Vector2 positionInGrid =
                _gridPositionCalculator.GetTileCenterWorldPosition(_grid,
                                                                   gridCoordinates.Value);
            transform.position = new Vector3(positionInGrid.x, positionInGrid.y, transform.position.z);
        }
    }
}
