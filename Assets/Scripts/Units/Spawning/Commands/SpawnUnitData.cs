using System;
using System.Runtime.Serialization;
using Math;

namespace Units.Commands {
    [Serializable]
    public class SpawnUnitData : ISerializable {
        public readonly UnitCommandData unitCommandData;
        public readonly IntVector2 tileCoords;
        public readonly bool isInitialSpawn;

        public SpawnUnitData(UnitCommandData unitCommandData, IntVector2 tileCoords, bool isInitialSpawn) {
            this.unitCommandData = unitCommandData;
            this.tileCoords = tileCoords;
            this.isInitialSpawn = isInitialSpawn;
        }
        
        #region ISerializable
        public SpawnUnitData(SerializationInfo info, StreamingContext context) {
            unitCommandData = (UnitCommandData)info.GetValue("unitCommandData", typeof(UnitCommandData));
            tileCoords = (IntVector2)info.GetValue("tileCoords", typeof(IntVector2));
            isInitialSpawn = info.GetBoolean("isInitialSpawn");
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("unitCommandData", unitCommandData);
            info.AddValue("tileCoords", tileCoords);
            info.AddValue("isInitialSpawn", isInitialSpawn);
        }
        #endregion 
    }
}