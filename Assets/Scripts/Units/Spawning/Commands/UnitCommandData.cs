using System;
using System.Runtime.Serialization;
using Units.Serialized;

namespace Units.Spawning.Commands {
    /// <summary>
    /// Serializable data that is used for commands performed on a unit.
    /// </summary>
    [Serializable]
    public class UnitCommandData : ISerializable {
        public readonly UnitId unitId;
        public readonly UnitDataReference unitDataReference;
        public readonly UnitCommandData[] pets;
        
        public uint UnitIndex => unitDataReference.UnitIndex;
        public UnitType UnitType => unitDataReference.UnitType;

        public UnitCommandData(UnitId unitId, uint unitIndex, UnitType unitType) : this(unitId,
                                                                                        unitIndex,
                                                                                        unitType,
                                                                                        new UnitCommandData[0]) { }

        public UnitCommandData(UnitId unitId, uint unitIndex, UnitType unitType, UnitCommandData[] pets) {
            this.unitId = unitId;
            this.unitDataReference = new UnitDataReference(unitIndex, unitType);
            this.pets = pets;
        }

        #region ISerializable
        public UnitCommandData(SerializationInfo info, StreamingContext context) {
            unitDataReference = (UnitDataReference) info.GetValue("unitDataReference", typeof(UnitDataReference));
            unitId = (UnitId) info.GetValue("unitId", typeof(UnitId));
            pets = (UnitCommandData[]) info.GetValue("pets", typeof(UnitCommandData[]));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("unitDataReference", unitDataReference);
            info.AddValue("unitId", unitId);
            info.AddValue("pets", pets);
        }
        #endregion
    }
}