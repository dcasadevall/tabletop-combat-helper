using System.Runtime.Serialization;
using Math;
using Units.Serialized;

namespace Units.Commands {
    /// <summary>
    /// Serializable data that is used for commands performed on a unit.
    /// </summary>
    public class UnitCommandData : ISerializable {
        public readonly UnitId unitId;
        public readonly UnitType unitType;
        public readonly uint unitIndex;
        public readonly UnitCommandData[] pets;

        public UnitCommandData(UnitId unitId, uint unitIndex, UnitType unitType) : this(unitId,
                                                                                        unitIndex,
                                                                                        unitType,
                                                                                        new UnitCommandData[0]) { }

        public UnitCommandData(UnitId unitId, uint unitIndex, UnitType unitType, UnitCommandData[] pets) {
            this.unitId = unitId;
            this.unitIndex = unitIndex;
            this.unitType = unitType;
            this.pets = pets;
        }
        
        #region ISerializable
        public UnitCommandData(SerializationInfo info, StreamingContext context) {
            unitIndex = info.GetUInt32("unitIndex");
            unitType = (UnitType)info.GetUInt32("unitType");
            unitId = (UnitId) info.GetValue("unitId", typeof(UnitId));
            pets = (UnitCommandData[])info.GetValue("pets", typeof(UnitCommandData[]));
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("unitIndex", unitIndex);
            info.AddValue("unitType", (int)unitType);
            info.AddValue("unitId", unitId);
            info.AddValue("pets", pets);
        }
        #endregion 
    }
}