using System.Runtime.Serialization;

namespace Drawing.Commands {
    public class ClearAllPixelsCommandData : ISerializable {
        public ClearAllPixelsCommandData() {
        }
        
        #region ISerializable
        public ClearAllPixelsCommandData(SerializationInfo info, StreamingContext context) {
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
        }
        #endregion  
    }
}