using System.Runtime.Serialization;

namespace CommandSystem {
    public interface ICommandQueueListener {
        void HandleCommandQueued<TData>(TData data) where TData : ISerializable;
    }
}