using CommandSystem;
using CommandSystem.Installers;

namespace Map.Commands {
    public class MapCommandsInstaller : AbstractCommandsInstaller {
        protected override void InstallCommandBindings() {
            // We must expose the concrete command for the typed creation to work.
            Container.Bind<LoadMapCommand>().AsSingle();
        }
    }
}