using Zenject;

namespace CommandSystem.Installers {
    /// <summary>
    /// This installer should be used when binding commands on an installer that will be used in scene contexts.
    /// It ensures that the <see cref="CommandFactory"/> has visibility over the scene context in order to resolve
    /// such commands.
    /// </summary>
    public abstract class AbstractCommandsInstaller : Installer {
        private readonly CommandFactory _commandFactory;

        public AbstractCommandsInstaller(CommandFactory commandFactory) {
            _commandFactory = commandFactory;
        }
        
        // This should NOT be called in the base method
        public override void InstallBindings() {
            _commandFactory.RegisterSceneContainer(Container);
            
            InstallCommandBindings();
        }

        protected abstract void InstallCommandBindings();
    }
}