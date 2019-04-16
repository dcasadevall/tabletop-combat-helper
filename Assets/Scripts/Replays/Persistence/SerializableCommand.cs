using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using CommandSystem;

namespace Replays.Persistence {
    /// <summary>
    /// Data structure used to serialize a single <see cref="CommandSystem.ICommandSnapshot"/>.
    /// </summary>
    [Serializable]
    public class SerializableCommand : ISerializable {
        public readonly ISerializable data;
        public readonly Type commandType;
        public readonly Type dataType;

        public SerializableCommand(ICommandSnapshot commandSnapshot) {
            data = commandSnapshot.Data;
            dataType = commandSnapshot.Data.GetType();
            commandType = commandSnapshot.Command.GetType();
        }

        #region ISerializable
        public SerializableCommand(SerializationInfo info, StreamingContext context) {
            data = (ISerializable)info.GetValue("data", typeof(ISerializable));
            dataType = (Type)info.GetValue("dataType", typeof(Type));
            commandType = (Type)info.GetValue("commandType", typeof(Type));
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("data", data);
            info.AddValue("dataType", dataType);
            info.AddValue("commandType", commandType);
        }
        #endregion  
    }
}