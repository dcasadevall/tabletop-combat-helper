using CommandSystem;
using CommandSystem.Installers;
using Units.Spawning;
using Zenject;

namespace Grid.Commands {
    public class GridCommandsInstaller : Installer {
        public override void InstallBindings() {
            // We expose the concrete type so it can be instantiated by Type.
            Container.Bind<MoveUnitCommand>().AsSingle();
            Container.Bind<RotateUnitCommand>().AsSingle();
            
            // For now, let RotateUnitCommand modify the unit transform directly, but this should go through
            // GridUnitManager which should be the only actor modifying it.
            Container.Bind<IUnitTransformRegistry>()
                     .To<UnitRegistry>()
                     .FromResolve()
                     .WhenInjectedInto<RotateUnitCommand>();
        }
    }
}