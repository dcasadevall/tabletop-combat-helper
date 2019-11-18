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
            
            // TODO: UnitPool to implement IMemoryPool
            Container.Bind<IUnitPool>().To<UnitPool>().AsSingle();
            Container.Bind<GameObject>().FromInstance(_unitPrefab).WhenInjectedInto<UnitPool>();

            // Prototype: Bind ITicker and IInitializable to the UnitsSpawner
            Container.BindInterfacesTo<UnitSpawner>().AsSingle();
            
            // Commands installer
            Container.Install<UnitCommandsInstaller>();

            // Unit Actions
            Container.Install<UnitActionsInstaller>();
        }
    }
}
