using CommandSystem.Installers;
using Units.Serialized;
using Units.Spawning.Commands;
using Units.Spawning.UI;
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
        
        [SerializeField]
        private GameObject _unitPickerVcPrefab;
        
        // Unit Spawn Settings is loaded in a preload scene
        // and injected here.
        private UnitSpawnSettings _unitSpawnSettings;
        
        [Inject]
        public void Construct(UnitSpawnSettings unitSpawnSettings) {
            _unitSpawnSettings = unitSpawnSettings;
        }
        
        public override void InstallBindings() {
            // Unit Spawning UI. This is injected here because the dependency graph is local to unit spawning.
            Container.Bind<IUnitSpawnViewController>().To<UnitSpawnViewController>()
                     .FromComponentInNewPrefab(_unitPickerVcPrefab)
                     .AsSingle()
                     .Lazy();
            
            // Unit Pool
            // NOTE: We do not use UnderTransformGroup because there is an issue with parenting under a
            // transform with a given name when switching scenes.
            // We should use UnderTransform instead, but that requires a specific Transform reference.
            Container.BindMemoryPool<UnitRenderer, UnitRenderer.Pool>()
                     .WithInitialSize(10)
                     .FromComponentInNewPrefab(_unitPrefab)
                     .WhenInjectedInto<UnitPool>();
            
            Container.Bind<IUnitPool>().To<UnitPool>().AsSingle();
            // Maybe use identifiers? random number?
            Container.Bind<IMutableUnitRegistry>().To<UnitRegistry>().FromResolve().WhenInjectedInto<UnitPool>();
            
            // Bind ITicker and IInitializable to the UnitsSpawner
            Container.BindInterfacesTo<PlayerUnitSpawner>().AsSingle();
            
            // Settings
            Container.Bind<IUnitSpawnSettings>().To<UnitSpawnSettings>().FromInstance(_unitSpawnSettings).AsSingle();
                        
            // Commands installer
            CommandsInstaller.Install<SpawnUnitCommandsInstaller>(Container);
        }
    }
}