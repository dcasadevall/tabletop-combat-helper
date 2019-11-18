using CommandSystem;
using Grid.Commands;
using Units.Actions;
using Units.Commands;
using Units.Serialized;
using Units.Spawning;
using Units.UI;
using UnityEngine;
using Zenject;

namespace Units {
    public class UnitsInstaller : MonoInstaller {
        [SerializeField]
        public GameObject _unitPickerViewController;
        [SerializeField]
        public GameObject _unitPrefab;

        // Unit Spawn Settings is loaded in a preload scene
        // and injected here.
        private UnitSpawnSettings _unitSpawnSettings;
        
        [Inject]
        public void Construct(UnitSpawnSettings unitSpawnSettings) {
            _unitSpawnSettings = unitSpawnSettings;
        }
        
        public override void InstallBindings() {
            // UI
            Container.Bind<IUnitPickerViewController>().FromComponentInNewPrefab(_unitPickerViewController).AsSingle();
            Container.Bind<ITickable>().To<UnitSelectionDetector>().AsSingle();
            
            Container.Bind<IUnitSpawnSettings>().To<UnitSpawnSettings>().FromInstance(_unitSpawnSettings).AsSingle();
            Container.Bind<IUnitDataIndexResolver>().To<UnitDataIndexResolver>().AsSingle();

            // TODO: Avoid having to expose UnitRegistry.
            Container.Bind<UnitRegistry>().AsSingle();
            Container.Bind<IUnitRegistry>().To<UnitRegistry>().FromResolve();
            Container.Bind<IMutableUnitRegistry>().To<UnitRegistry>().FromResolve().WhenInjectedInto<UnitPool>();
            
            // Unit Pool
            Container.BindMemoryPool<UnitRenderer, UnitRenderer.Pool>().WithInitialSize(1)
                     .FromComponentInNewPrefab(_unitPrefab).WhenInjectedInto<UnitPool>();
            Container.Bind<IUnitPool>().To<UnitPool>().AsSingle();

            // Prototype: Bind ITicker and IInitializable to the UnitsSpawner
            Container.BindInterfacesTo<UnitSpawner>().AsSingle();
            
            // Commands installer
            Container.Install<UnitCommandsInstaller>();

            // Unit Actions
            Container.Install<UnitActionsInstaller>();
        }
    }
}
