using System.Runtime.Serialization;

namespace CommandSystem {
    public interface ICommandQueue {
        void Enqueue(ICommand command);
        void Enqueue<TData>(ICommand<TData> command, TData data) where TData : ISerializable;
    }
}