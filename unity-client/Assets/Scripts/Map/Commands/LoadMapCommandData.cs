using System;
using System.Runtime.Serialization;

namespace Map.Commands {
    [Serializable]
    public class LoadMapCommandData : ISerializable {
        public uint mapIndex;

        public LoadMapCommandData(uint mapIndex) {
            this.mapIndex = mapIndex;
        }
        
        #region ISerializable
        public LoadMapCommandData(SerializationInfo info, StreamingContext context) {
            mapIndex = info.GetUInt32("mapIndex");
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("mapIndex", mapIndex);
        }
        #endregion  
    }
}