using System;
using System.Runtime.Serialization;
using Math;
using Units;

namespace Grid.Commands {
    [Serializable]
    public class RotateUnitData : ISerializable {
        public readonly UnitId unitId;
        public readonly int degrees;
        
        public RotateUnitData(UnitId unitId, int degrees) {
            this.unitId = unitId;
            this.degrees = degrees;
        }
        
        #region ISerializable
        public RotateUnitData(SerializationInfo info, StreamingContext context) {
            unitId = (UnitId)info.GetValue("unitId", typeof(UnitId));
            degrees = info.GetInt32("degrees");
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("unitId", unitId);
            info.AddValue("degrees", degrees);
        }
        #endregion  
    }
}