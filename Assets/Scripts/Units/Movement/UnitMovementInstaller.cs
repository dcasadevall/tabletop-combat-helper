using Grid.Commands;
using Units.Actions;
using Units.Movement.ActionHandlers;
using Units.Spawning;
using Zenject;

namespace Units.Movement {
    public class UnitMovementInstaller : Installer {
        public override void InstallBindings() {
            // Movement Controller
            Container.Bind<IUnitMovementController>().To<UnitMovementController>().AsSingle();
            
            // Action Handlers
            Container.Bind<ISingleUnitActionHandler>().To<UnitDestinationSelector>().AsSingle();
            Container.Bind<ISingleUnitActionHandler>().To<UnitPathPlanner>().AsSingle();
            Container.Bind<ISingleUnitActionHandler>().To<UnitMoveAnimator>().AsSingle();
            Container.Bind(typeof(ISingleUnitActionHandler), typeof(IBatchedUnitActionHandler))
                     .To<UnitDragAndDropHandler>()
                     .AsSingle();
            
                        
            // UnitDragAndDropHandler modifies transform to preview unit location.
            Container.Bind<IUnitTransformRegistry>()
                     .To<UnitRegistry>()
                     .FromResolve()
                     .WhenInjectedInto<UnitDragAndDropHandler>();
        }
    }
}