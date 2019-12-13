using System;
using System.Runtime.Serialization;
using Math;
using Units;

namespace Grid.Commands {
    [Serializable]
    public class MoveUnitData : ISerializable {
        public readonly UnitId unitId;
        public readonly IntVector2 moveDistance;

        public MoveUnitData(UnitId unitId, IntVector2 moveDistance) {
            this.unitId = unitId;
            this.moveDistance = moveDistance;
        }
        
        #region ISerializable
        public MoveUnitData(SerializationInfo info, StreamingContext context) {
            unitId = (UnitId)info.GetValue("unitId", typeof(UnitId));
            moveDistance = (IntVector2) info.GetValue("moveDistance", typeof(IntVector2));
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("unitId", unitId);
            info.AddValue("moveDistance", moveDistance);
        }
        #endregion 
    }
}