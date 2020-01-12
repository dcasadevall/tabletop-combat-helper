using UnityEngine;
using Zenject;

namespace Units.Spawning {
    /// <summary>
    /// <see cref="MonoBehaviour"/> responsible for binding serialized fields found in the unit prefab to their
    /// respective unit data.
    /// </summary>
    public class UnitRenderer : MonoBehaviour, IPoolable {
#pragma warning disable 649
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        [SerializeField]
        private SpriteRenderer _avatarIconRenderer;
        [SerializeField]
        private Animator _animator;
#pragma warning restore 649

        #region IPoolable
        // Something is really messed up and the pool inactive items are enabled by default...
        // so we need to do this.
        public void OnDespawned() {
            gameObject.SetActive(false);
        }

        public void OnSpawned() {
            gameObject.SetActive(true);
        }
        #endregion

        public void SetSelected(bool selected) {
            _animator.SetBool("Selected", selected);
        }

        internal void SetUnit(IUnit unit) {
            _spriteRenderer.sprite = unit.UnitData.Sprite;
            _avatarIconRenderer.sprite = unit.UnitData.AvatarSprite; 
        }

        internal class Pool : MonoMemoryPool<UnitRenderer> {
        }
    }
}