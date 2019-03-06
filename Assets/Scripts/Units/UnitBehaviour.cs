using System.Collections.Generic;
using Prototype;
using Units.Serialized;
using UnityEngine;
using Zenject;

namespace Units {
    /// <summary>
    /// <see cref="MonoBehaviour"/> responsible for binding serialized fields found in the unit prefab to their
    /// respective unit data.
    /// </summary>
    public class UnitBehaviour : MonoBehaviour {
#pragma warning disable 649
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        [SerializeField]
        private SpriteRenderer _avatarIconRenderer; 
#pragma warning restore 649
        
        public void SetUnitData(IUnitData unitData) {
            _spriteRenderer.sprite = unitData.Sprite;
            _avatarIconRenderer.sprite = unitData.AvatarSprite; 
        }
        
        public class Pool : MonoMemoryPool<IUnitData, UnitBehaviour> {
            protected override void Reinitialize(IUnitData unitData, UnitBehaviour unitBehaviour) {
                unitBehaviour.SetUnitData(unitData);
            }
        }
    }
}