using System.Runtime.Serialization;

namespace CommandSystem {
    public interface ICommandFactory {
        ICommand<TData> Create<TData>() where TData : ISerializable;
    }
}