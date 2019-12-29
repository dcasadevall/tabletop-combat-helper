using Units.Actions.Handlers.Move;
using Zenject;

namespace Units.Actions {
    public class UnitActionsInstaller : Installer {
        public override void InstallBindings() {
            Container.Bind<IUnitActionPlanner>().To<UnitActionBroadcaster>().AsSingle();
            
            // Action Listeners
            Container.Bind<IUnitActionHandler>().To<UnitDestinationSelector>().AsSingle();
            Container.Bind<IUnitActionHandler>().To<UnitDragAndDropHandler>().AsSingle();
            Container.Bind<IUnitActionHandler>().To<UnitPathPlanner>().AsSingle();
            Container.Bind<IUnitActionHandler>().To<UnitMoveAnimator>().AsSingle();
        }
    }
}