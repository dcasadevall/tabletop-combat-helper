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
            unitCommandData = new UnitCommandData(info, context);
            tileCoords = new IntVector2(info, context);
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            unitCommandData.GetObjectData(info, context);
            tileCoords.GetObjectData(info, context);
        }
        #endregion 
    }
}