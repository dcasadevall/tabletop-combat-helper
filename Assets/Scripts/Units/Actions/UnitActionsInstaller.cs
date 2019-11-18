using Units.Actions.Listeners.Move;
using Zenject;

namespace Units.Actions {
    public class UnitActionsInstaller : Installer {
        public override void InstallBindings() {
            Container.Bind<IUnitActionPlanner>().To<UnitActionBroadcaster>().AsSingle();
            
            // Action Listeners
            Container.Bind<IUnitActionListener>().To<UnitGridPositionPreviewer>();
            Container.Bind<IUnitActionListener>().To<UnitValidMovementHighlighter>();
        }
    }
}