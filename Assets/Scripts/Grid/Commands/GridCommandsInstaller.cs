using CommandSystem;
using CommandSystem.Installers;
using Units.Spawning;
using Zenject;

namespace Grid.Commands {
    public class GridCommandsInstaller : AbstractCommandsInstaller {
        public GridCommandsInstaller(ICommandBinder commandBinder) : base(commandBinder) { }
        
        public override void InstallBindings() {
            // We bind this command as transient so it can store state for Undo()
            // Also, we expose the concrete type so it can be instantiated by Type.
            BindCommand<MoveUnitCommand>(binder => binder.AsTransient());
            BindCommand<RotateUnitCommand>(binder => binder.AsSingle());
            
            // For now, let RotateUnitCommand modify the unit transform directly, but this should go through
            // GridUnitManager which should be the only actor modifying it.
            Container.Bind<IUnitTransformRegistry>()
                     .To<UnitRegistry>()
                     .FromResolve()
                     .WhenInjectedInto<RotateUnitCommand>();
        }
    }
}