namespace Zenject {
    /// <summary>
    /// Installer used in project context for global project bindings.
    /// </summary>
    public class ProjectInstaller : MonoInstaller {
        public override void InstallBindings() {
            SignalBusInstaller.Install(Container);
        }
    }
}