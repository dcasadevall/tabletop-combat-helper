using Zenject;

namespace CommandSystem.Installers {
    /// <summary>
    /// This installer should be used when binding commands on an installer that will be used in scene contexts.
    /// It ensures that the <see cref="CommandFactory"/> has visibility over the scene context in order to resolve
    /// such commands.
    /// </summary>
    public abstract class AbstractCommandsInstaller : Installer {
        // This should NOT be called in the base method
        public override void InstallBindings() {
            CommandFactory.RegisterSceneContainer(Container);
            
            InstallCommandBindings();
        }

        protected abstract void InstallCommandBindings();
    }
}