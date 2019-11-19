using Units.Serialized;
using Units.Spawning.Commands;
using UnityEngine;
using Zenject;

namespace Units.Spawning {
    /// <summary>
    /// Installer used for spawning units.
    /// This is separate from <see cref="UnitsInstaller"/> because we need to keep our unit pool
    /// and spawned units relative the scenes we spawn units in.
    /// i.e: If there are two map sections, each one of them has a separate unit pool.
    /// </summary>
    public class UnitSpawningInstaller : MonoInstaller {
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
            // Unit Pool
            Container.BindMemoryPool<UnitRenderer, UnitRenderer.Pool>().WithInitialSize(1)
                     .FromComponentInNewPrefab(_unitPrefab).WhenInjectedInto<UnitPool>();
            Container.Bind<IUnitPool>().To<UnitPool>().AsSingle();
            Container.Bind<IMutableUnitRegistry>().To<UnitRegistry>().FromResolve().WhenInjectedInto<UnitPool>();
            
            // Bind ITicker and IInitializable to the UnitsSpawner
            Container.BindInterfacesTo<UnitSpawner>().AsSingle();
            
            // Settings
            Container.Bind<IUnitSpawnSettings>().To<UnitSpawnSettings>().FromInstance(_unitSpawnSettings).AsSingle();
                        
            // Commands installer
            Container.Install<SpawnUnitCommandsInstaller>();
        }
    }
}