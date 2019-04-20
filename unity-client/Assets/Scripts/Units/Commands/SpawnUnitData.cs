using System;
using System.Runtime.Serialization;
using Math;

namespace Units.Commands {
    [Serializable]
    public class SpawnUnitData : ISerializable {
        public readonly UnitCommandData unitCommandData;
        public readonly IntVector2 tileCoords;

        public SpawnUnitData(UnitCommandData unitCommandData, IntVector2 tileCoords) {
            this.unitCommandData = unitCommandData;
            this.tileCoords = tileCoords;
        }
        
        #region ISerializable
        public SpawnUnitData(SerializationInfo info, StreamingContext context) {
            unitCommandData = (UnitCommandData)info.GetValue("unitCommandData", typeof(UnitCommandData));
            tileCoords = (IntVector2)info.GetValue("tileCoords", typeof(IntVector2));
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("unitCommandData", unitCommandData);
            info.AddValue("tileCoords", tileCoords);
        }
        #endregion 
    }
}