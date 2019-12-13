using CommandSystem;
using CommandSystem.Installers;
using Units.Serialized;
using Zenject;

namespace Units.Spawning.Commands {
    public class SpawnUnitCommandsInstaller : Installer {
        public override void InstallBindings() {
            Container.Bind<IMutableUnitRegistry>().To<UnitRegistry>().FromResolve().WhenInjectedInto<SpawnUnitCommand>();

            Container.Bind<SpawnUnitCommand>().AsSingle();
            Container.Bind<IFactory<IUnitData, UnitCommandData>>().To<UnitCommandDataFactory>().AsSingle();
        }
    }
}