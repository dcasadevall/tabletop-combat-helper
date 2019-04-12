using System;
using System.Runtime.Serialization;

namespace CommandSystem {
    public delegate void CommandQueued(ICommand<ISerializable> command, ISerializable data);
    
    public interface ICommandQueue {
        event CommandQueued commandQueued;

        void Enqueue<TCommand, TData>(TData data) where TData : ISerializable
                                                  where TCommand : ICommand<TData>;
    }
    
    public static class CommandQueueExtensions {
        public static void Enqueue<TCommand, TData>(this ICommandQueue commandQueue) where TData : ISerializable, new() 
                                                                                     where TCommand : ICommand<TData> {
            commandQueue.Enqueue<TCommand, TData>(new TData());
        }
    }
}