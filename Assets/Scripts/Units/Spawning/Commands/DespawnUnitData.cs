using System.Runtime.Serialization;

namespace Units.Spawning.Commands {
    public class DespawnUnitData : ISerializable {
        public readonly UnitId unitId;

        public DespawnUnitData(UnitId unitId) {
            this.unitId = unitId;
        }

        #region ISerializable
        public DespawnUnitData(SerializationInfo info, StreamingContext context) {
            unitId = (UnitId) info.GetValue("unitId", typeof(UnitId));
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("unitId", unitId);
        }
        #endregion  
    }
}