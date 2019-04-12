using System;
using System.Runtime.Serialization;

namespace CommandSystem {
    public interface ICommandFactory {
        ICommand<ISerializable> Create(Type dataType);
    }
}