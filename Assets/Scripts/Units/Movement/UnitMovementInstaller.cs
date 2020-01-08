using Units.Actions;
using Units.Movement.ActionHandlers;
using Units.Movement.Handlers;
using Zenject;

namespace Units.Movement {
    public class UnitMovementInstaller : Installer {
        public override void InstallBindings() {
            // Movement Controller
            Container.Bind<IUnitMovementController>().To<UnitMovementController>().AsSingle();
            
            // Action Handlers
            Container.Bind<IUnitActionHandler>().To<UnitDestinationSelector>().AsSingle();
            Container.Bind<IUnitActionHandler>().To<UnitDragAndDropHandler>().AsSingle();
            Container.Bind<IUnitActionHandler>().To<UnitPathPlanner>().AsSingle();
            Container.Bind<IUnitActionHandler>().To<UnitMoveAnimator>().AsSingle();
        }
    }
}