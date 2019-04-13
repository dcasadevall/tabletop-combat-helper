using System;
using System.Runtime.Serialization;

namespace CommandSystem {
    public interface ICommandFactory {
        ICommand<TData> Create<TData>() where TData : ISerializable;
    }

    public static class CommandFactoryExtensions {
        /// <summary>
        /// This method is used to reconstruct the command queue via serialized commands.
        /// We may want to rethink how our command interface works if it gets real slow
        /// with reflection here..
        /// </summary>
        /// <param name="commandFactory"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public static ICommand<ISerializable> Create(this ICommandFactory commandFactory, Type dataType) {
            var methodInfo = commandFactory.GetType().GetMethod("Create");
            var genericMethod = methodInfo.MakeGenericMethod(dataType);
            return (ICommand<ISerializable>)genericMethod.Invoke(commandFactory, new object[0]);
        }
    }
}