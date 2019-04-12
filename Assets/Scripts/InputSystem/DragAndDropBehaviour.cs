using System;
using CommandSystem;
using Grid.Commands;
using Grid.Positioning;
using Math;
using Units;
using Units.Commands;
using Units.Serialized;
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

        private ICommandQueue _commandQueue;
        private IGridPositionCalculator _gridPositionCalculator;
        private IInputLock _inputLock;
        private Guid? _lockId;
        private Camera _camera;
        private UnitId _unitId;

        [Inject]
        public void Construct(ICommandQueue commandQueue,
                              IGridPositionCalculator gridPositionCalculator, IInputLock inputLock,
                              Camera camera) {
            _camera = camera;
            _commandQueue = commandQueue;
            _inputLock = inputLock;
            _gridPositionCalculator = gridPositionCalculator;
        }

        public void SetUnitId(UnitId unitId) {
            _unitId = unitId;
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

            MoveUnitData moveUnitData = new MoveUnitData(_unitId, gridCoordinates.Value);
            _commandQueue.Enqueue<MoveUnitCommand, MoveUnitData>(moveUnitData);
        }
    }
}
