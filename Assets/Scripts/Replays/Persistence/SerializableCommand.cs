using System;
using System.Runtime.Serialization;
using CommandSystem;

namespace Replays.Persistence {
    /// <summary>
    /// Data structure used to serialize a single <see cref="CommandSystem.ICommandSnapshot"/>.
    /// </summary>
    [Serializable]
    public class SerializableCommand : ISerializable {
        public readonly ISerializable data;
        public readonly Type type;

        public SerializableCommand(ICommandSnapshot commandSnapshot) {
            data = commandSnapshot.Data;
            type = commandSnapshot.Type;
        }

        #region ISerializable
        public SerializableCommand(SerializationInfo info, StreamingContext context) {
            data = (ISerializable)info.GetValue("data", typeof(ISerializable));
            type = (Type)info.GetValue("type", typeof(Type));
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("data", data);
            info.AddValue("type", type);
        }
        #endregion  
    }
}