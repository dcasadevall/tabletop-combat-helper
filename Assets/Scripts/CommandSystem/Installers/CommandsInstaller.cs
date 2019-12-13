using System;
using System.Collections.Generic;
using Zenject;

namespace CommandSystem.Installers {
    /// <summary>
    /// This installer should be used when binding commands on an installer that will be used in scene contexts.
    /// It ensures that the <see cref="CommandFactory"/> has visibility in order to resolve
    /// such commands, after each command is bound via <see cref="CommandFactory#BindCommand()"/>
    /// </summary>
    public class CommandsInstaller {
        /// <summary>
        /// This method should be called in order to install an installer containing <see cref="ICommand"/>s
        /// </summary>
        public static void Install<TInstaller>(DiContainer container) where TInstaller : Installer {
            container.Install<TInstaller>();
            CommandFactory.RegisterSceneContainer(container);
        }
    }
}