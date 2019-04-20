using System;
using System.Runtime.Serialization;

namespace Networking.Messaging {
    [Serializable]
    public class NetworkMessage : ISerializable {
        public readonly Guid playerId;
        public readonly byte[] data;
        public readonly short tag;
        
        public NetworkMessage(Guid playerId, byte[] data, short tag) {
            this.playerId = playerId;
            this.data = data;
            this.tag = tag;
        }
        
        #region ISerializable
        public NetworkMessage(SerializationInfo info, StreamingContext context) {
            playerId = new Guid(info.GetString("playerId"));
            data = (byte[]) info.GetValue("data", typeof(byte[]));
            tag = info.GetInt16("tag");
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("playerId", playerId.ToString());
            info.AddValue("data", data);
            info.AddValue("tag", tag);
        }
        #endregion 
    }
}