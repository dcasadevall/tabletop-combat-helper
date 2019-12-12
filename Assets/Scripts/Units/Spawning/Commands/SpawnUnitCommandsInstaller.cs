using CommandSystem;
using CommandSystem.Installers;
using Units.Serialized;
using Zenject;

namespace Units.Spawning.Commands {
    public class SpawnUnitCommandsInstaller : CommandsInstaller {
        public SpawnUnitCommandsInstaller(CommandFactory commandFactory) : base(commandFactory) { }
        
        public override void InstallBindings() {
            Container.Bind<IMutableUnitRegistry>().To<UnitRegistry>().FromResolve().WhenInjectedInto<SpawnUnitCommand>();

            BindCommand<SpawnUnitCommand>().AsSingle();
            Container.Bind<IFactory<IUnitData, UnitCommandData>>().To<UnitCommandDataFactory>().AsSingle();
        }
    }
}