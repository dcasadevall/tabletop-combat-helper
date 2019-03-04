using System;
using System.Collections.Generic;
using Units.Serialized;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace Prototype {
    /// <summary>
    /// <see cref="NetworkBehaviour"/> used to sync units across clients.
    /// It is used to sync the unit index when a unit is spawned, allowing clients
    /// to populate each unit's data (sprites, initial values, etc..) 
    /// </summary>
    public class UnitNetworkBehaviour : NetworkBehaviour {
        public class Pool : MonoMemoryPool<int, UnitNetworkBehaviour> {
            protected override void Reinitialize(int unitIndex, UnitNetworkBehaviour unitNetworkBehaviour) {
                unitNetworkBehaviour.OnUnitIndexChanged(unitIndex);
            }
        }
        
        public SpriteRenderer spriteRenderer;
        public SpriteRenderer avatarIconRenderer;
        
        // [HideInInspector]
        // [SyncVar(hook="OnUnitIndexChanged")]
        public Int32 unitIndex;

        private List<IUnitData> _unitDatas;

        [Inject]
        public void Construct(List<IUnitData> unitDatas) {
            _unitDatas = unitDatas;
        }

        private void OnUnitIndexChanged(int newUnitIndex) {
            unitIndex = newUnitIndex;
            if (newUnitIndex >= _unitDatas.Count) {
                return;
            }

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