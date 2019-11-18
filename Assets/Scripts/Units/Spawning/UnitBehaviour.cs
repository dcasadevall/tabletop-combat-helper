using System.Collections.Generic;
using Grid;
using Grid.Positioning;
using InputSystem;
using Logging;
using Math;
using UnityEngine;
using Zenject;
using ILogger = Logging.ILogger;

namespace Units.Spawning {
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
        private IGridPositionCalculator _gridPositionCalculator;

        [Inject]
        public void Construct(IGrid grid, IGridPositionCalculator gridPositionCalculator) {
            _gridPositionCalculator = gridPositionCalculator;
        }

        private void HandleUnitPlacedAtTile(IUnit unit, IntVector2 tileCoords) {
            if (unit.UnitId != _unitId) {
                return;
            }
            
            Vector2 worldPosition = _gridPositionCalculator.GetTileCenterWorldPosition(tileCoords);
            transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
        }

        internal void SetUnit(IUnit unit) {
            _unitId = unit.UnitId;
            _spriteRenderer.sprite = unit.UnitData.Sprite;
            _avatarIconRenderer.sprite = unit.UnitData.AvatarSprite; 
        }
    }
}