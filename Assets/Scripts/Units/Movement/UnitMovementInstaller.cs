using Units.Actions;
using Units.Movement.Handlers;
using Zenject;

namespace Units.Movement {
    public class UnitMovementInstaller : Installer {
        private readonly IUnitActionPlanner _unitActionPlanner;
        public UnitMovementInstaller(IUnitActionPlanner unitActionPlanner) {
            _unitActionPlanner = unitActionPlanner;
        }

        public override void InstallBindings() {
            // Movement Controller
            Container.Bind<IUnitMovementController>().To<UnitMovementController>().AsSingle();
            Container.Bind<IUnitActionPlanner>()
                     .FromInstance(_unitActionPlanner)
                     .WhenInjectedInto<UnitMovementController>();
            
            // Action Handlers
            Container.Bind<IUnitActionHandler>().To<UnitDestinationSelector>().AsSingle();
            Container.Bind<IUnitActionHandler>().To<UnitDragAndDropHandler>().AsSingle();
            Container.Bind<IUnitActionHandler>().To<UnitPathPlanner>().AsSingle();
            Container.Bind<IUnitActionHandler>().To<UnitMoveAnimator>().AsSingle();
        }
    }
}