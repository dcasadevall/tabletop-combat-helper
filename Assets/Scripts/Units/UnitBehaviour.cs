using System.Collections.Generic;
using Grid;
using Grid.Positioning;
using InputSystem;
using Logging;
using Math;
using UnityEngine;
using ILogger = Logging.ILogger;
using Zenject;

namespace Units {
    /// <summary>
    /// <see cref="MonoBehaviour"/> responsible for binding serialized fields found in the unit prefab to their
    /// respective unit data.
    ///
    /// Additionally, this class handles animations and transform modifications when a unit is placed / moved
    /// around the tile. 
    /// </summary>
    public class UnitBehaviour : MonoBehaviour {
#pragma warning disable 649
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        [SerializeField]
        private SpriteRenderer _avatarIconRenderer;
        // TODO: This will go away with unit selection
        [SerializeField]
        private DragAndDropBehaviour _dragAndDropBehaviour;
#pragma warning restore 649

        private UnitId _unitId;
        private IGridUnitManager _gridUnitManager;
        private IGridPositionCalculator _gridPositionCalculator;

        [Inject]
        public void Construct(IGrid grid, IGridUnitManager gridUnitManager,
                              IGridPositionCalculator gridPositionCalculator) {
            _gridUnitManager = gridUnitManager;
            _gridPositionCalculator = gridPositionCalculator;
        }

        private void HandleUnitPlacedAtTile(IUnit unit, IntVector2 tileCoords) {
            if (unit.UnitId != _unitId) {
                return;
            }
            
            Vector2 worldPosition = _gridPositionCalculator.GetTileCenterWorldPosition(tileCoords);
            transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
        }

        private void SetUnit(IUnit unit) {
            _unitId = unit.UnitId;
            _spriteRenderer.sprite = unit.UnitData.Sprite;
            _avatarIconRenderer.sprite = unit.UnitData.AvatarSprite; 
            _dragAndDropBehaviour.SetUnitId(unit.UnitId);
            _gridUnitManager.UnitPlacedAtTile += HandleUnitPlacedAtTile;
        }
        
        private void HandleDespawn() {
            _gridUnitManager.UnitPlacedAtTile -= HandleUnitPlacedAtTile;
        }
        
        public class Pool : MonoMemoryPool<IUnit, UnitBehaviour> {
            Dictionary<UnitId, UnitBehaviour> _unitBehaviours = new Dictionary<UnitId, UnitBehaviour>();
            private readonly ILogger _logger;

            public Pool(ILogger logger) {
                _logger = logger;
            }
            
            protected override void Reinitialize(IUnit unit, UnitBehaviour unitBehaviour) {
                _unitBehaviours[unit.UnitId] = unitBehaviour;
                unitBehaviour.SetUnit(unit);
            }

            protected override void OnDespawned(UnitBehaviour unitBehaviour) {
                unitBehaviour.HandleDespawn();
                base.OnDespawned(unitBehaviour);
            }

            public void Despawn(UnitId unitId) {
                if (!_unitBehaviours.ContainsKey(unitId)) {
                    _logger.LogError(LoggedFeature.Units, "Despawn called on unitId not found: {0}", unitId);
                    return;
                }
                
                Despawn(_unitBehaviours[unitId]);
                _unitBehaviours.Remove(unitId);
            }
        }
    }
}