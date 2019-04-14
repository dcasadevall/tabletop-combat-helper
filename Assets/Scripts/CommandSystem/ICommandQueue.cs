using System;
using System.Runtime.Serialization;

namespace CommandSystem {
    public interface ICommandQueue {
        void AddListener(ICommandQueueListener listener);
        void Enqueue(Type commandType, Type dataType, ISerializable data);
    }
    
    public static class CommandQueueExtensions {
        public static void Enqueue<TCommand, TData>(this ICommandQueue commandQueue, ISerializable data)
            where TCommand : class, ICommand
            where TData : ISerializable {
            commandQueue.Enqueue(typeof(TCommand), typeof(TData), data);
        }

        public static void Enqueue<TCommand, TData>(this ICommandQueue commandQueue) where TCommand : class, ICommand 
                                                                                     where TData : ISerializable, new() {
            commandQueue.Enqueue(typeof(TCommand), typeof(TData), new TData());
        }
    }
}