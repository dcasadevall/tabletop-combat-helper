using CommandSystem;
using CommandSystem.Installers;
using Units.Serialized;
using Zenject;

namespace Units.Commands {
    public class UnitCommandsInstaller : AbstractCommandsInstaller {
        public UnitCommandsInstaller(CommandFactory commandFactory) : base(commandFactory) { }
        
        protected override void InstallCommandBindings() {
            Container.Bind<IMutableUnitRegistry>().To<UnitRegistry>().FromResolve().WhenInjectedInto<SpawnUnitCommand>();
            Container.Bind<ICommand<SpawnUnitData>>().To<SpawnUnitCommand>().AsSingle();
            Container.Bind<IFactory<IUnitData, UnitCommandData>>().To<UnitCommandDataFactory>().AsSingle();
        }
    }
}