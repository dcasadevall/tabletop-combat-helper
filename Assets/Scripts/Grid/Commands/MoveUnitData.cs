using System.Runtime.Serialization;
using Math;
using Units;
using Units.Commands;

namespace Grid.Commands {
    public class MoveUnitData : ISerializable {
        public readonly UnitId unitId;
        public readonly IntVector2 tileCoords;

        public MoveUnitData(UnitId unitId, IntVector2 tileCoords) {
            this.unitId = unitId;
            this.tileCoords = tileCoords;
        }
        
        #region ISerializable
        public MoveUnitData(SerializationInfo info, StreamingContext context) {
            unitId = new UnitId(info, context);
            tileCoords = new IntVector2(info, context);
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            unitId.GetObjectData(info, context);
            tileCoords.GetObjectData(info, context);
        }
        #endregion 
    }
}