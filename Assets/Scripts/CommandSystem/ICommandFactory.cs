
using System;
using System.Runtime.Serialization;

namespace CommandSystem {
    public interface ICommandFactory {
        ICommand Create(Type type, Type dataType, ISerializable data);
    }

    public static class CommandFactoryExtensions {
        public static ICommand Create<TCommand, TData>(this ICommandFactory commandFactory, TData data)
            where TData : ISerializable {
            return commandFactory.Create(typeof(TCommand), typeof(TData), data);
        }
    }
}