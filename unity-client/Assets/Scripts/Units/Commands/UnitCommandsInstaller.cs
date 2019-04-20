using CommandSystem;
using CommandSystem.Installers;
using Units.Serialized;
using Units.Spawning;
using Zenject;

namespace Units.Commands {
    public class UnitCommandsInstaller : AbstractCommandsInstaller {
        public UnitCommandsInstaller(CommandFactory commandFactory) : base(commandFactory) { }
        
        protected override void InstallCommandBindings() {
            Container.Bind<IMutableUnitRegistry>().To<UnitRegistry>().FromResolve().WhenInjectedInto<SpawnUnitCommand>();

            // We must expose the concrete command for the typed creation to work.
            Container.Bind<SpawnUnitCommand>().AsSingle();
            Container.Bind<IFactory<IUnitData, UnitCommandData>>().To<UnitCommandDataFactory>().AsSingle();
        }
    }
}