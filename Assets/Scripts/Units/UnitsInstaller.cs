using UI;
using Units.Actions;
using Units.Movement;
using Units.Selection;
using Units.Serialized;
using Units.Spawning;
using UnityEngine;
using Zenject;

namespace Units {
    public class UnitsInstaller : MonoInstaller {
        public override void InstallBindings() {
            // Unit Data
            Container.Bind<IUnitDataIndexResolver>().To<UnitDataIndexResolver>().AsSingle();

            // TODO: Avoid having to expose UnitRegistry.
            // This is used by installers that want to expose IUnitTransformRegistry to some first class citizens.
            Container.Bind<UnitRegistry>().AsSingle();
            Container.Bind<IUnitRegistry>().To<UnitRegistry>().FromResolve();

            Container.Install<UnitActionsInstaller>();
            Container.Install<UnitMovementInstaller>();
        }
    }
}
