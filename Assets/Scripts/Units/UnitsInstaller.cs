using CommandSystem;
using Grid.Commands;
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

        public override void InstallBindings() {
            // UI
            Container.Bind<IUnitPickerViewController>().FromComponentInNewPrefab(_unitPickerViewController).AsSingle();
            Container.Bind<ITickable>().To<UnitSelectionDetector>().AsSingle();
            
            Container.Bind<IUnitDataIndexResolver>().To<UnitDataIndexResolver>().AsSingle();

            // TODO: Avoid having to expose UnitRegistry.
            Container.Bind<UnitRegistry>().AsSingle();
            Container.Bind<IUnitRegistry>().To<UnitRegistry>().FromResolve();

            // Unit Actions
            Container.Install<UnitActionsInstaller>();
        }
    }
}
