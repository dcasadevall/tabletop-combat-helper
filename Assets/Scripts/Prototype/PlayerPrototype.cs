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
    public class PlayerPrototype : NetworkBehaviour {
        public SpriteRenderer spriteRenderer;
        public SpriteRenderer avatarIconRenderer;
        
        // [HideInInspector]
        // [SyncVar(hook="OnUnitIndexChanged")]
//        [SyncVar]
        public Int32 unitIndex;

        private IUnitData[] _unitDatas;

        private void Start() {
            _unitDatas = FindObjectOfType<PlayerPrototypeDI>().unitDatas;
        }

        private void Update() {
            OnUnitIndexChanged(unitIndex);
        }

        public void OnUnitIndexChanged(int newUnitIndex) {
            unitIndex = newUnitIndex;
            Debug.Log("Set Unit index (Before): " + unitIndex);
            _unitDatas = FindObjectOfType<PlayerPrototypeDI>().unitDatas;
            if (_unitDatas == null) {
                return;
            }
            
            Debug.Log("Set Unit index: " + unitIndex);
            if (newUnitIndex >= _unitDatas.Length) {
                return;
            }

            // Note: this is never called.
            spriteRenderer.sprite = _unitDatas[newUnitIndex].Sprite;
            avatarIconRenderer.sprite = _unitDatas[newUnitIndex].AvatarSprite;
        }
        
         
        public const uint BIT_CANBEDAMAGED = 1u,
                          BIT_CANBEHEALED = 2u,
                          BIT_CANBEREVIVED = 4u;
   public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        bool e = base.OnSerialize(writer, initialState);
        if (initialState)
        {
            writer.Write(unitIndex);
 
            return true;
        }
 
        writer.Write(syncVarDirtyBits);
 
        bool wroteSyncVar = false;  
 
        if ((syncVarDirtyBits & BIT_CANBEDAMAGED) != 0u)
        {
            wroteSyncVar = true;
            writer.Write(unitIndex);
        } 
 
        return e || wroteSyncVar;
    }
 
    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        base.OnDeserialize(reader, initialState);
 
        if (initialState)
        {
            unitIndex = reader.ReadInt32();
 
             return;
        }
 
         Int32 _syncVarDirtyBits = reader.ReadInt32();
 
         if ((_syncVarDirtyBits & BIT_CANBEDAMAGED) != 0u)
         {
             unitIndex = reader.ReadInt32();
//             if (OnCanBeDamagedChanged != null)
//                 OnCanBeDamagedChanged(this);
         }
    }
    }
}