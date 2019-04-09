using CommandSystem;
using Zenject;

namespace Units.Commands {
    public class UnitCommandsInstaller : Installer {
        public override void InstallBindings() {
            Container.Bind<ICommand<SpawnUnitData>>().To<SpawnUnitCommand>().FromSubContainerResolve()
                     .ByMethod(BindSpawnUnitCommand).AsSingle();
        }

        private void BindSpawnUnitCommand(DiContainer container) {
            container.Bind<SpawnUnitCommand>().AsSingle();
            container.Bind<IMutableUnitRegistry>().To<UnitRegistry>().FromResolve();
        }
        
    }
}