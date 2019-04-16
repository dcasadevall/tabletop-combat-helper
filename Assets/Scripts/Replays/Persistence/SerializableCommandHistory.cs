using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using CommandSystem;

namespace Replays.Persistence {
    /// <summary>
    /// Data structure used to serialize / deserialize a series of command snapshots.
    /// </summary>
    [Serializable]
    public class SerializableCommandHistory : ISerializable {
        private List<SerializableCommand> _commands = new List<SerializableCommand>();
        public IEnumerable<SerializableCommand> Commands {
            get {
                return _commands;
            }
        }
        
        public SerializableCommandHistory() {
        }
        
        #region ISerializable
        public SerializableCommandHistory(SerializationInfo info, StreamingContext context) {
            _commands = (List<SerializableCommand>) info.GetValue("commands", typeof(List<SerializableCommand>));
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("commands", _commands);
        }
        #endregion  
        
        public void AddCommandSnapshot(ICommandSnapshot commandSnapshot) {
            _commands.Add(new SerializableCommand(commandSnapshot));
        }

    }
}