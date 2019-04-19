using System;
using CommandSystem;
using Grid;
using Grid.Commands;
using Grid.Positioning;
using Math;
using Units;
using Units.Commands;
using Units.Serialized;
using Units.Spawning;
using UnityEngine;
using Zenject;

namespace InputSystem {
    /// <summary>
    /// Quick and dirty little behaviour to drag and drop 2d objects around the game.
    /// This will eventually be refactored
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class DragAndDropBehaviour : MonoBehaviour {
        private ICommandQueue _commandQueue;
        private IUnitRegistry _unitRegistry;
        private IGridUnitManager _gridUnitManager;
        private IGridPositionCalculator _gridPositionCalculator;
        private IInputLock _inputLock;
        private Guid? _lockId;
        private Camera _camera;
        private UnitId _unitId;
        private Vector3 _offset;
        private IntVector2? _previousCoordinates;

        [Inject]
        public void Construct(ICommandQueue commandQueue,
                              IUnitRegistry unitRegistry,
                              IGridUnitManager gridUnitManager,
                              IGridPositionCalculator gridPositionCalculator, IInputLock inputLock,
                              Camera camera) {
            _camera = camera;
            _commandQueue = commandQueue;
            _inputLock = inputLock;
            _unitRegistry = unitRegistry;
            _gridUnitManager = gridUnitManager;
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

            IUnit unit = _unitRegistry.GetUnit(_unitId);
            _previousCoordinates = _gridUnitManager.GetUnitCoords(unit);
            _offset = gameObject.transform.position - _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
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
            Vector3 curPosition = _camera.ScreenToWorldPoint(curScreenPoint) + _offset;
            IntVector2? gridCoordinates = _gridPositionCalculator.GetTileContainingWorldPosition(curPosition);
            
            if (gridCoordinates == null) {
                return;
            }

            // Make sure we only send a move command if necessary (tile hasn't changed)
            if (_previousCoordinates != null && _previousCoordinates.Value == gridCoordinates.Value) {
                return;
            }
            _previousCoordinates = gridCoordinates;
            
            MoveUnitData moveUnitData = new MoveUnitData(_unitId, gridCoordinates.Value);
            _commandQueue.Enqueue<MoveUnitCommand, MoveUnitData>(moveUnitData);
        }
    }
}
