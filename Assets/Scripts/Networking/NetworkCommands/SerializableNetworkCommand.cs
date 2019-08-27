using System;
using System.Runtime.Serialization;
using Replays.Persistence;

namespace Networking.NetworkCommands {
    [Serializable]
    public class SerializableNetworkCommand : ISerializable {
        private readonly uint _sequenceIndex;
        private SerializableCommand _serializableCommand;

        public SerializableNetworkCommand(uint sequenceIndex, SerializableCommand command) {
            _sequenceIndex = sequenceIndex;
            _serializableCommand = command;
        }
        
        #region ISerializable
        public SerializableNetworkCommand(SerializationInfo info, StreamingContext context) {
            _sequenceIndex = info.GetUInt32("sequenceIndex");
            _serializableCommand = (SerializableCommand)info.GetValue("command", typeof(SerializableCommand));
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("sequenceIndex", _sequenceIndex);
            info.AddValue("command", _serializableCommand);
        }
        #endregion  
    }
}