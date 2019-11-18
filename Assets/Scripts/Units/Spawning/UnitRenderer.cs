using UnityEngine;
using Zenject;

namespace Units.Spawning {
    /// <summary>
    /// <see cref="MonoBehaviour"/> responsible for binding serialized fields found in the unit prefab to their
    /// respective unit data.
    /// </summary>
    public class UnitRenderer : MonoBehaviour {
#pragma warning disable 649
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        [SerializeField]
        private SpriteRenderer _avatarIconRenderer;
#pragma warning restore 649

        internal void SetUnit(IUnit unit) {
            _spriteRenderer.sprite = unit.UnitData.Sprite;
            _avatarIconRenderer.sprite = unit.UnitData.AvatarSprite; 
        }

        internal class Pool : MemoryPool<UnitRenderer> {
        }
    }
}