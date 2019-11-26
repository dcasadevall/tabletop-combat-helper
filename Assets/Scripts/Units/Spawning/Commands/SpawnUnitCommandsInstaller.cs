using CommandSystem;
using CommandSystem.Installers;
using Units.Serialized;
using Zenject;

namespace Units.Spawning.Commands {
    public class SpawnUnitCommandsInstaller : AbstractCommandsInstaller {
        protected override void InstallCommandBindings() {
            Container.Bind<IMutableUnitRegistry>().To<UnitRegistry>().FromResolve().WhenInjectedInto<SpawnUnitCommand>();

            // We must expose the concrete command for the typed creation to work.
            Container.Bind<SpawnUnitCommand>().AsSingle();
            Container.Bind<IFactory<IUnitData, UnitCommandData>>().To<UnitCommandDataFactory>().AsSingle();
        }
    }
}