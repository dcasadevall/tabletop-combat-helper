using System;
using System.Collections.Generic;
using Zenject;

namespace CommandSystem.Installers {
    /// <summary>
    /// This installer should be used when binding commands on an installer that will be used in scene contexts.
    /// It ensures that the <see cref="CommandFactory"/> has visibility in order to resolve
    /// such commands, after each command is bound via <see cref="CommandFactory#BindCommand()"/>
    /// </summary>
    public abstract class CommandsInstaller : Installer {
        protected readonly CommandBinder _commandBinder;

        internal CommandsInstaller(CommandBinder commandBinder) {
            _commandBinder = commandBinder;
        }
    }
}