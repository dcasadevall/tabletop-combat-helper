using CommandSystem;
using CommandSystem.Installers;
using Units.Serialized;
using Zenject;

namespace Units.Spawning.Commands {
    public class SpawnUnitCommandsInstaller : AbstractCommandsInstaller {
        public SpawnUnitCommandsInstaller(ICommandBinder commandBinder) : base(commandBinder) { }
        
        public override void InstallBindings() {
            Container.Bind<IMutableUnitRegistry>().To<UnitRegistry>().FromResolve().WhenInjectedInto<SpawnUnitCommand>();

            BindCommand<SpawnUnitCommand>(binder => binder.AsSingle());
            Container.Bind<IFactory<IUnitData, UnitCommandData>>().To<UnitCommandDataFactory>().AsSingle();
        }
    }
}