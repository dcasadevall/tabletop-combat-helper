
using System;
using System.Runtime.Serialization;

namespace CommandSystem {
    public interface ICommandFactory {
        ICommand Create(Type type, Type dataType, ISerializable data);
    }
}