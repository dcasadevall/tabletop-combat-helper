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
        private RadialMenuViewController _unitMenuPrefab;
        
        public override void InstallBindings() {
            // UI
            Container.Bind<IUnitPickerViewController>().FromComponentInNewPrefab(_unitPickerViewController).AsSingle();
            Container.Bind<ITickable>().To<UnitSelectionDetector>().AsSingle();
            
            // UI: Radial Menu. For now, no need to use ID since there is only 1 radial menu.
            Container.Bind<IRadialMenu>().To<RadialMenuViewController>()
                     .FromComponentInNewPrefab(_unitMenuPrefab)
                     .AsSingle()
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
