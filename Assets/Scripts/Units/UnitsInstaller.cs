using Units.Actions;
using Units.Editing;
using Units.Movement;
using Units.Selection;
using Units.Serialized;
using Units.Spawning;
using UnityEngine;
using Zenject;

namespace Units {
    public class UnitsInstaller : MonoInstaller {
        [SerializeField]
        private GameObject _unitEditingPrefab;

        [SerializeField]
        private UnitMenuViewController _unitMenuPrefab;
        
        public override void InstallBindings() {
            // UI: Selection prefab. Inject into the installer so we avoid having too many MonoInstallers,
            // while being able to isolate dependencies.
            Container.Bind<UnitMenuViewController>()
                     .FromComponentInNewPrefab(_unitMenuPrefab)
                     .AsSingle()
                     .WhenInjectedInto<UnitSelectionInstaller>()
                     .Lazy();
            
            // UI: Unit Editing / Picking. Inject into installer as we do with other unit menus.
            Container.Bind<UnitEditingViewController>()
                     .FromComponentInNewPrefab(_unitEditingPrefab)
                     .AsSingle()
                     .WhenInjectedInto<UnitEditingInstaller>()
                     .Lazy();

            // Unit Data
            Container.Bind<IUnitDataIndexResolver>().To<UnitDataIndexResolver>().AsSingle();

            // TODO: Avoid having to expose UnitRegistry.
            Container.Bind<UnitRegistry>().AsSingle();
            Container.Bind<IUnitRegistry>().To<UnitRegistry>().FromResolve();

            Container.Install<UnitActionsInstaller>();
            Container.Install<UnitMovementInstaller>();
            Container.Install<UnitSelectionInstaller>();
        }
    }
}
