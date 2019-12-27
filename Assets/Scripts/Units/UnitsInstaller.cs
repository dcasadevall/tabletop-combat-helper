using CommandSystem;
using Grid.Commands;
using UI.RadialMenu;
using Units.Actions;
using Units.Serialized;
using Units.Spawning;
using Units.Spawning.Commands;
using Units.UI;
using UnityEngine;
using Zenject;

namespace Units {
    public class UnitsInstaller : MonoInstaller {
        [SerializeField]
        public GameObject _unitPickerViewController;

        [SerializeField]
        private UnitMenuViewController _unitMenuPrefab;
        
        [SerializeField]
        private MoveUnitMenuViewController _moveUnitMenuPrefab;
        
        public override void InstallBindings() {
            // UI
            Container.Bind<IUnitPickerViewController>().FromComponentInNewPrefab(_unitPickerViewController).AsSingle();
            Container.Bind<ITickable>().To<UnitSelectionDetector>().AsSingle();
            
            // UI: Unit Menu
            Container.Bind<UnitMenuViewController>()
                     .FromComponentInNewPrefab(_unitMenuPrefab)
                     .AsSingle()
                     .WhenInjectedInto<UnitSelectionDetector>()
                     .Lazy();
            
            Container.Bind<MoveUnitMenuViewController>()
                     .FromComponentInNewPrefab(_moveUnitMenuPrefab)
                     .AsSingle()
                     .WhenInjectedInto<UnitMenuViewController>()
                     .Lazy();

            Container.Bind<IUnitDataIndexResolver>().To<UnitDataIndexResolver>().AsSingle();

            // TODO: Avoid having to expose UnitRegistry.
            Container.Bind<UnitRegistry>().AsSingle();
            Container.Bind<IUnitRegistry>().To<UnitRegistry>().FromResolve();

            // Unit Actions
            Container.Install<UnitActionsInstaller>();
        }
    }
}
