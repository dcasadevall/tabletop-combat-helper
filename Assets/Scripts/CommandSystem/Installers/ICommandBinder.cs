using System;
using Zenject;

namespace CommandSystem.Installers {
    public interface ICommandBinder {
        void BindCommand<TCommand>(DiContainer container,
                                   Action<ConcreteIdBinderGeneric<ICommand>> afterBind,
                                   Func<bool> isActiveFunc) where TCommand : ICommand;
    }
}