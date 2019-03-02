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
        
        [SyncVar(hook="OnUnitIndexChanged")]
        public uint unitIndex;

        private IUnitData[] _unitDatas;

        private void Awake() {
            _unitDatas = GetComponent<PlayerPrototypeDI>().unitDatas;
        }
        
/*        public override void OnStartClient() {
            base.OnStartClient();

            OnUnitIndexChanged(unitIndex);
        }*/

        private void OnUnitIndexChanged(uint newUnitIndex) {
            spriteRenderer.sprite = _unitDatas[newUnitIndex].Sprite;
        }
    }
}