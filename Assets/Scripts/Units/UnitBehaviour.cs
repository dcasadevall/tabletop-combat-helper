using Grid;
using Grid.Positioning;
using Math;
using UnityEngine;
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
#pragma warning restore 649

        private UnitId _unitId;
        private IGridUnitManager _gridUnitManager;
        private IGridPositionCalculator _gridPositionCalculator;
        private IGrid _grid;

        [Inject]
        public void Construct(IGrid grid, IGridUnitManager gridUnitManager,
                              IGridPositionCalculator gridPositionCalculator) {
            _grid = grid;
            _gridUnitManager = gridUnitManager;
            _gridPositionCalculator = gridPositionCalculator;
            _gridUnitManager.UnitPlacedAtTile += HandleUnitPlacedAtTile;
        }

        private void HandleUnitPlacedAtTile(IUnit unit, IntVector2 tileCoords) {
            if (unit.UnitId != _unitId) {
                return;
            }
            
            Vector2 worldPosition = _gridPositionCalculator.GetTileCenterWorldPosition(_grid, tileCoords);
            transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
        }

        private void SetUnit(IUnit unit) {
            _unitId = unit.UnitId;
            _spriteRenderer.sprite = unit.UnitData.Sprite;
            _avatarIconRenderer.sprite = unit.UnitData.AvatarSprite; 
            _gridUnitManager.UnitPlacedAtTile += HandleUnitPlacedAtTile;
        }
        
        private void HandleDespawn() {
            _gridUnitManager.UnitPlacedAtTile -= HandleUnitPlacedAtTile;
        }
        
        public class Pool : MonoMemoryPool<IUnit, UnitBehaviour> {
            protected override void Reinitialize(IUnit unit, UnitBehaviour unitBehaviour) {
                unitBehaviour.SetUnit(unit);
            }

            protected override void OnDespawned(UnitBehaviour unitBehaviour) {
                unitBehaviour.HandleDespawn();
                base.OnDespawned(unitBehaviour);
            }
        }
    }
}