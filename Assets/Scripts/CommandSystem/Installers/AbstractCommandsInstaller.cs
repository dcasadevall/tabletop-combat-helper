using System;
using System.Collections.Generic;
using Zenject;

namespace CommandSystem.Installers {
    /// <summary>
    /// This installer should be used when binding commands on an installer that will be used in scene contexts.
    /// It ensures that the <see cref="CommandFactory"/> has visibility in order to resolve
    /// such commands, after each command is bound via <see cref="CommandFactory#BindCommand()"/>
    /// </summary>
    public abstract class AbstractCommandsInstaller : Installer {
        private readonly ICommandBinder _commandBinder;
        private Func<bool> _isEnabledFunc;

        /// <summary>
        /// This method should be called in order to install the commands installer with an "enabledFunc" checker
        /// other than the default <see cref="Installer.IsEnabled"/> function.
        /// This can be useful, for example, to check if a mono installer is enabled in order to determined
        /// that a commands installer should be enabled.
        /// </summary>
        public static void Install<TContract>(DiContainer container, Func<bool> isEnabledFunc) where TContract : AbstractCommandsInstaller {
            TContract installer = container.Instantiate<TContract>();
            installer._isEnabledFunc = isEnabledFunc;
            installer.InstallBindings();
        }

        internal AbstractCommandsInstaller(ICommandBinder commandBinder) {
            _commandBinder = commandBinder;
        }

        protected void BindCommand<TCommand>(Action<ConcreteIdBinderGeneric<ICommand>> afterBind)
            where TCommand : ICommand {
            var isEnabledFunc = _isEnabledFunc ?? (() => IsEnabled);
            _commandBinder.BindCommand<TCommand>(Container, afterBind, isEnabledFunc);
        }
    }
}