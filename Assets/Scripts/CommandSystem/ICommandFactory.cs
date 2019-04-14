
using System;

namespace CommandSystem {
    public interface ICommandFactory {
        ICommand Create<TCommand>() where TCommand : class, ICommand;
        ICommand Create(Type type);
    }
}