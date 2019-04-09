using CommandSystem;
using Zenject;

namespace Units.Commands {
    public class UnitCommandsInstaller : Installer {
        public override void InstallBindings() {
            // Commands
            Container.Bind<ICommand<SpawnUnitData>>().To<SpawnUnitCommand>().AsSingle();
        }
    }
}