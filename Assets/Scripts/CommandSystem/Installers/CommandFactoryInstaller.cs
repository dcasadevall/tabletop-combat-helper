using Zenject;

namespace CommandSystem.Installers {
    /// <summary>
    /// 
    /// </summary>
    public class CommandFactoryInstaller : Installer {
        private readonly CommandFactory _commandFactory;

        public CommandFactoryInstaller(CommandFactory commandFactory) {
            _commandFactory = commandFactory;
        }
        
        public override void InstallBindings() {
            // This assumes we have injected the CommandFactory via "WhenInjectedTo" onto this installer.
            Container.Bind<ICommandFactory>().To<CommandFactory>().FromInstance(_commandFactory);
            
            // This guarantees that CommandFactory is only exposed to installers derived from CommandsInstaller
            Container.Bind<CommandFactory>().FromInstance(_commandFactory)
                     .When(context => context.ObjectType.IsSubclassOf(typeof(CommandsInstaller)));
        }
    }
}