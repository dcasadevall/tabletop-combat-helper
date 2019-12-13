using CommandSystem.Installers;
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
            // NOTE: There is a bug in Zenject pools when we switch scenes and spawn elements in a pool while doing so.
            // Zenject will use the pool of the currently active scene, instead of the one in context.
            // To avoid that, for now, we tread this pool as one with a size of 1.
            Container.BindMemoryPool<UnitRenderer, UnitRenderer.Pool>()
                     .WithInitialSize(10)
                     .FromComponentInNewPrefab(_unitPrefab)
                     .WhenInjectedInto<UnitPool>();
            
            Container.Bind<IUnitPool>().To<UnitPool>().AsSingle();
            // Maybe use identifiers? random number?
            Container.Bind<IMutableUnitRegistry>().To<UnitRegistry>().FromResolve().WhenInjectedInto<UnitPool>();
            
            // Bind ITicker and IInitializable to the UnitsSpawner
            Container.BindInterfacesTo<UnitSpawner>().AsSingle();
            
            // Settings
            Container.Bind<IUnitSpawnSettings>().To<UnitSpawnSettings>().FromInstance(_unitSpawnSettings).AsSingle();
                        
            // Commands installer
            CommandsInstaller.Install<SpawnUnitCommandsInstaller>(Container);
        }
    }
}