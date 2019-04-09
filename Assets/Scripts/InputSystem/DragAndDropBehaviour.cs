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
        public event System.Action<Vector2> GridPositionChanged = delegate {};
        
        private Vector3 offset;

        private ICommandQueue _commandQueue;
        private ICommand<MoveUnitData> _moveUnitCommand;
        private IUnitDataIndexResolver _unitDataIndexResolver;
        private IGridPositionCalculator _gridPositionCalculator;
        private IInputLock _inputLock;
        private Guid? _lockId;
        private Camera _camera;
        private IUnit _unit;

        [Inject]
        public void Construct(ICommand<MoveUnitData> moveUnitCommand, ICommandQueue commandQueue,
                              IUnitDataIndexResolver unitDataIndexResolver,
                              IGridPositionCalculator gridPositionCalculator, IInputLock inputLock,
                              Camera camera) {
            _camera = camera;
            _commandQueue = commandQueue;
            _moveUnitCommand = moveUnitCommand;
            _unitDataIndexResolver = unitDataIndexResolver;
            _inputLock = inputLock;
            _gridPositionCalculator = gridPositionCalculator;
        }

        public void SetUnit(IUnit unit) {
            _unit = unit;
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
            
            uint unitIndex = _unitDataIndexResolver.ResolveUnitIndex()
            UnitCommandData unitCommandData = new UnitCommandData();
            MoveUnitData moveUnitData = new MoveUnitData();
            _commandQueue.Enqueue(_moveUnitCommand, );
        }
    }
}
