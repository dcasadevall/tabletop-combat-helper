using System;
using Grid.Positioning;
using Math;
using UnityEngine;
using Zenject;

namespace InputSystem {
    /// <summary>
    /// Quick and dirty little behaviour to drag and drop 2d objects around the game.
    /// This will eventually be refactored
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class DragAndDropBehaviour : MonoBehaviour {
        private Vector3 offset;
        
        private IGridPositionCalculator _gridPositionCalculator;
        private IInputLock _inputLock;
        private Guid? _lockId;
        private Camera _camera;

        [Inject]
        public void Construct(IGridPositionCalculator gridPositionCalculator, IInputLock inputLock,
                              Camera camera) {
            _camera = camera;
            _inputLock = inputLock;
            _gridPositionCalculator = gridPositionCalculator;
        }

        private void OnDestroy() {
            OnMouseUp();
        }

        private void OnMouseDown() {
            // Acquire input lock. If we fail to do so, return.
            _lockId = _inputLock.Lock();
            if (_lockId == null) {
                return;
            }

            offset = gameObject.transform.position - _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        }

        private void OnMouseUp() {
            if (_lockId == null) {
                return;
            }
            
            _inputLock.Unlock(_lockId.Value);
            _lockId = null;
        }

        private void OnMouseDrag() {
            // If we don't own the input lock, don't do anything.
            if (_lockId == null) {
                return;
            }
            
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
