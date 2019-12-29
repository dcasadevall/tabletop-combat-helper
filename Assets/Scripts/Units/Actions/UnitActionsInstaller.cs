using Units.Movement;
using Zenject;

namespace Units.Actions {
    public class UnitActionsInstaller : Installer {
        public override void InstallBindings() {
            Container.Bind<UnitActionBroadcaster>()
                     .FromSubContainerResolve()
                     .ByMethod(InstallUnitActionPlanner)
                     .AsSingle();
        }

        // Unit Action Planner is exposed to a few first class users.
        // I.e: UnitMovementController uses UnitActionPlanner, but does not expose it.
        private void InstallUnitActionPlanner(DiContainer container) {
            container.Bind<UnitActionBroadcaster>().AsSingle();
            container.Bind<IUnitActionPlanner>()
                     .To<UnitActionBroadcaster>()
                     .FromResolve()
                     .WhenInjectedInto<UnitMovementInstaller>();
        }
    }
}