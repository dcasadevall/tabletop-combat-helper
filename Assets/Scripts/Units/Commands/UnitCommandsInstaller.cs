using CommandSystem;
using Zenject;

namespace Units.Commands {
    public class UnitCommandsInstaller : Installer {
        public override void InstallBindings() {
            Container.Bind<IMutableUnitRegistry>().To<UnitRegistry>().FromResolve().WhenInjectedInto<SpawnUnitCommand>();
            Container.Bind<ICommand<SpawnUnitData>>().To<SpawnUnitCommand>().AsSingle();
        }
    }
}