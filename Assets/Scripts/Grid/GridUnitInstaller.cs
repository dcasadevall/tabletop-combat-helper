using CommandSystem.Installers;
using Grid.Commands;
using Units.Spawning;
using Zenject;

namespace Grid {
    public class GridUnitInstaller : MonoInstaller {
        public override void InstallBindings() {
            Container.Bind(typeof(IGridUnitManager), typeof(IInitializable)).To<GridUnitManager>().AsSingle();
            // Grid input manager depends on Units because it also exposes units under the mouse.
            // If needed, we should separate input vs unit input.
            Container.BindInterfacesTo<GridInputManager>().AsSingle();
            
            // GridUnitManager should be the only thing mutating the unit transform.
            Container.Bind<IUnitTransformRegistry>().To<UnitRegistry>().FromResolve().WhenInjectedInto<GridUnitManager>();
            
            // Commands should only be related to units in Grid.
            CommandsInstaller.Install<GridUnitCommandsInstaller>(Container);
        }
    }
}