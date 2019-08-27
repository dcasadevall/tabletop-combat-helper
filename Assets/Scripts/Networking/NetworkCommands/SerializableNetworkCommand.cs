using System;
using System.Runtime.Serialization;
using Replays.Persistence;

namespace Networking.NetworkCommands {
    [Serializable]
    public class SerializableNetworkCommand : ISerializable {
        public readonly uint sequenceIndex;
        public SerializableCommand command;

        public SerializableNetworkCommand(uint sequenceIndex, SerializableCommand command) {
            this.command = command;
            this.sequenceIndex = sequenceIndex;
        }
        
        #region ISerializable
        public SerializableNetworkCommand(SerializationInfo info, StreamingContext context) {
            sequenceIndex = info.GetUInt32("sequenceIndex");
            command = (SerializableCommand)info.GetValue("command", typeof(SerializableCommand));
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("sequenceIndex", sequenceIndex);
            info.AddValue("command", command);
        }
        #endregion  
    }
}