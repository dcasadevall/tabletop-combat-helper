using System.Runtime.Serialization;

namespace CommandSystem {
    public interface ICommandQueue {
        void AddListener(ICommandQueueListener listener);
        void Enqueue<TData>(TData data) where TData : ISerializable;
    }
    
    public static class CommandQueueExtensions {
        public static void Enqueue<TData>(this ICommandQueue commandQueue) where TData : ISerializable, new() {
            commandQueue.Enqueue(new TData());
        }
    }
}