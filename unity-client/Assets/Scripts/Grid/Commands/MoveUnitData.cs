using System;
using System.Runtime.Serialization;
using Math;
using Units;

namespace Grid.Commands {
    [Serializable]
    public class MoveUnitData : ISerializable {
        public readonly UnitId unitId;
        public readonly IntVector2 tileCoords;

        public MoveUnitData(UnitId unitId, IntVector2 tileCoords) {
            this.unitId = unitId;
            this.tileCoords = tileCoords;
        }
        
        #region ISerializable
        public MoveUnitData(SerializationInfo info, StreamingContext context) {
            unitId = (UnitId)info.GetValue("unitId", typeof(UnitId));
            tileCoords = (IntVector2) info.GetValue("tileCoords", typeof(IntVector2));
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("unitId", unitId);
            info.AddValue("tileCoords", tileCoords);
        }
        #endregion 
    }
}