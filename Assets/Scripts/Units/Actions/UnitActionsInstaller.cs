using Units.Actions.Handlers.Move;
using Zenject;

namespace Units.Actions {
    public class UnitActionsInstaller : Installer {
        public override void InstallBindings() {
            Container.Bind<IUnitActionPlanner>().To<UnitActionBroadcaster>().AsSingle();
            
            // Action Listeners
            Container.Bind<IUnitActionHandler>().To<UnitDestinationSelector>().AsSingle();
        }
    }
}