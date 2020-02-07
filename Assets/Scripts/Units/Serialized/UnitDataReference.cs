using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace Units.Serialized {
    [Serializable]
    public class UnitDataReference : ISerializable {
        [SerializeField]
        private uint _unitIndex;
        public uint UnitIndex => _unitIndex;

        [SerializeField]
        private UnitType _unitType;
        public UnitType UnitType => _unitType;

        public UnitDataReference(uint unitIndex, UnitType unitType) {
            _unitIndex = unitIndex;
            _unitType = unitType;
        }
        
        #region ISerializable
        public UnitDataReference(SerializationInfo info, StreamingContext context) {
            _unitIndex = info.GetUInt32("unitIndex");
            _unitType = (UnitType)info.GetUInt32("unitType");
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("unitIndex", _unitIndex);
            info.AddValue("unitType", (int)_unitType);
        }
        #endregion

        public override string ToString() {
            return $"[Unit Index: {_unitIndex}, Unit Type: {_unitType}]";
        }
    }
}