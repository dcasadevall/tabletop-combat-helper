using CommandSystem;
using CommandSystem.Installers;
using Zenject;

namespace Map.Commands {
    public class MapCommandsInstaller : Installer {
        public override void InstallBindings() {
            Container.Bind<LoadMapCommand>().AsSingle();
        }
    }
}