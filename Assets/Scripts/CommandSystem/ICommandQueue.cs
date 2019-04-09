using System;
using System.Runtime.Serialization;

namespace CommandSystem {
    public interface ICommandQueue {
        void Enqueue<TData>(TData data) where TData : ISerializable;
    }

    public static class ICommandQueueExtensions {
        public static void Enqueue<TData>(this ICommandQueue commandQueue) where TData : ISerializable, new() {
            commandQueue.Enqueue(new TData());
        }
    }
}