using System;
using System.Runtime.Serialization;
using Units;

namespace Grid.Commands {
    [Serializable]
    public class MoveUnitSectionCommandData : ISerializable {
        public readonly UnitId unitId;
        public readonly uint fromSectionIndex;
        public readonly uint toSectionIndex;

        public MoveUnitSectionCommandData(UnitId unitId, uint fromSectionIndex, uint toSectionIndex) {
            this.unitId = unitId;
            this.fromSectionIndex = fromSectionIndex;
            this.toSectionIndex = toSectionIndex;
        }

        #region ISerializable
        public MoveUnitSectionCommandData(SerializationInfo info, StreamingContext context) {
            unitId = (UnitId) info.GetValue("unitId", typeof(UnitId));
            fromSectionIndex = (uint) info.GetInt32("fromSectionIndex");
            toSectionIndex = (uint) info.GetInt32("toSectionIndex");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("unitId", unitId);
            info.AddValue("fromSectionIndex", fromSectionIndex);
            info.AddValue("toSectionIndex", toSectionIndex);
        }
        #endregion
    }
}