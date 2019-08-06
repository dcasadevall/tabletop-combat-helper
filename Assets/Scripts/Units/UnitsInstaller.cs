using CommandSystem;
using Units.Commands;
using Units.Serialized;
using Units.Spawning;
using Units.UI;
using UnityEngine;
using Zenject;

namespace Units {
    public class UnitsInstaller : MonoInstaller {
        [SerializeField]
        public UnitSpawnSettings _unitSpawnSettings;
        [SerializeField]
        public GameObject _unitPickerViewController;
        [SerializeField]
        public GameObject _unitPrefab;
        
        public override void InstallBindings() {
            Container.Bind<IUnitPickerViewController>().FromComponentInNewPrefab(_unitPickerViewController).AsSingle();
            Container.Bind<IUnitSpawnSettings>().To<UnitSpawnSettings>().FromInstance(_unitSpawnSettings).AsSingle();
            Container.Bind<IUnitDataIndexResolver>().To<UnitDataIndexResolver>().AsSingle();

            // TODO: Avoid having to expose UnitRegistry.
            Container.Bind<UnitRegistry>().AsSingle();
            Container.Bind<IUnitRegistry>().To<UnitRegistry>().FromResolve();

            // TODO: This initial size is 1 because of a race condition when switching map sections and spawning units.
            // If there is space in the current section's pool, when switching to a new section, that section will
            // spawn units in the previous section pool.
            Container.BindMemoryPool<UnitBehaviour, UnitBehaviour.Pool>().WithInitialSize(1)
                     .FromComponentInNewPrefab(_unitPrefab).UnderTransformGroup("UnitPool");

            // Prototype: Bind ITicker and IInitializable to the UnitsSpawner
            Container.BindInterfacesTo<UnitSpawner>().AsSingle();
            
            // Commands installer
            Container.Install<UnitCommandsInstaller>();

        }
    }
}
