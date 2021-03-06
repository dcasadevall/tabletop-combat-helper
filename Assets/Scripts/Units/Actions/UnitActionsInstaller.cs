using Units.Movement;
using Units.Selection;
using Zenject;

namespace Units.Actions {
    public class UnitActionsInstaller : Installer {
        public override void InstallBindings() {
            Container.Bind<IUnitActionPlanner>()
                     .FromSubContainerResolve()
                     .ByMethod(InstallUnitActionPlanner)
                     .AsSingle();
        }

        // Unit Action Planner is exposed to a few first class citizens.
        // I.e: UnitMovementController uses UnitActionPlanner, but does not expose it.
        // This method should only inject IUnitActionPlanner to the few that need it.
        private void InstallUnitActionPlanner(DiContainer container) {
            container.Bind<UnitActionBroadcaster>().AsSingle();
            container.Bind<IUnitActionPlanner>()
                     .To<UnitActionBroadcaster>()
                     .FromResolve()
                     .WhenInjectedInto<UnitMovementController>();
            container.Bind<IUnitActionPlanner>()
                     .To<UnitActionBroadcaster>()
                     .FromResolve()
                     .WhenInjectedInto<BatchUnitMenuViewController>();
        }
    }
}