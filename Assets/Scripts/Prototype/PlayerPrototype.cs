using System;
using Ninject;
using Ninject.Unity;
using Units.Serialized;
using UnityEngine;
using UnityEngine.Networking;

namespace Prototype {
    /// <summary>
    /// Temp PlayerPrototype to sync the sprite loaded.
    /// </summary>
    [RequireComponent(typeof(PlayerPrototypeDI))]
    public class PlayerPrototype : NetworkBehaviour {
        public SpriteRenderer spriteRenderer;
        public SpriteRenderer avatarIconRenderer;
        
        [HideInInspector]
        [SyncVar(hook="OnUnitIndexChanged")]
        public uint unitIndex;

        private IUnitData[] _unitDatas;

        private void Awake() {
            _unitDatas = GetComponent<PlayerPrototypeDI>().unitDatas;
        }

        private void OnUnitIndexChanged(uint newUnitIndex) {
            // Note: this is never called.
            spriteRenderer.sprite = _unitDatas[newUnitIndex].Sprite;
            avatarIconRenderer.sprite = _unitDatas[newUnitIndex].AvatarSprite;
        }
    }
}