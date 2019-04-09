using System.Runtime.Serialization;
using Math;
using Units;
using Units.Commands;

namespace Grid.Commands {
    public class MoveUnitData : ISerializable {
        public readonly UnitCommandData unitCommandData;
        public readonly IntVector2 tileCoords;

        public MoveUnitData(UnitCommandData unitCommandData, IntVector2 tileCoords) {
            this.unitCommandData = unitCommandData;
            this.tileCoords = tileCoords;
        }
        
        #region ISerializable
        public MoveUnitData(SerializationInfo info, StreamingContext context) {
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