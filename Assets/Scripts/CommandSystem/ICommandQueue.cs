using System;
using System.Runtime.Serialization;

namespace CommandSystem {
    public interface ICommandQueue {
        void AddListener(ICommandQueueListener listener);
        void Enqueue(Type commandType, ISerializable data);
    }
    
    public static class CommandQueueExtensions {
        public static void Enqueue<TCommand>(this ICommandQueue commandQueue, ISerializable data) where TCommand : class, ICommand {
            commandQueue.Enqueue(typeof(TCommand), data);
        }
    }
}