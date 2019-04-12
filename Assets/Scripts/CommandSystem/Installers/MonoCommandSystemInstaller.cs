using Zenject;

namespace CommandSystem.Installers {
    /// <summary>
    /// Until we refactor our installers so they don't rely on monoinstallers, we are using this as a way
    /// to reuse the CommandSystemInstaller, while keeping the MonoInstaller that can be used in project context.
    /// </summary>
    public class MonoCommandSystemInstaller : MonoInstaller {
        public override void InstallBindings() {
            Container.Install<CommandSystemInstaller>();
        }
    }
}