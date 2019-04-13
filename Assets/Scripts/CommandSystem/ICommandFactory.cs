using System;
using System.Runtime.Serialization;

namespace CommandSystem {
    public interface ICommandFactory {
        ICommand<TData> Create<TData>() where TData : ISerializable;
        ICommand Create(Type commandType);
    }
}