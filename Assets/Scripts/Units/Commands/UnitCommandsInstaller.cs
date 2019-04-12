using CommandSystem;
using CommandSystem.Installers;
using Zenject;

namespace Units.Commands {
    public class UnitCommandsInstaller : AbstractCommandsInstaller {
        public UnitCommandsInstaller(CommandFactory commandFactory) : base(commandFactory) { }
        
        protected override void InstallCommandBindings() {
            Container.Bind<IMutableUnitRegistry>().To<UnitRegistry>().FromResolve().WhenInjectedInto<SpawnUnitCommand>();
            Container.Bind<SpawnUnitCommand>().AsSingle();
        }
    }
}