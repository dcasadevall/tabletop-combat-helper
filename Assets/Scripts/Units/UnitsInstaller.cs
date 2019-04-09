using CommandSystem;
using Units.Commands;
using Units.Serialized;
using Units.UI;
using UnityEngine;
using Zenject;

namespace Units {
    public class UnitsInstaller : MonoInstaller {
        public UnitSpawnSettings unitSpawnSettingses;
        public GameObject unitPickerViewController;
        public GameObject unitPrefab;
        
        public override void InstallBindings() {
            Container.Bind<IUnitPickerViewController>().FromComponentInNewPrefab(unitPickerViewController).AsSingle();
            Container.Bind<IUnitSpawnSettings>().To<UnitSpawnSettings>().FromInstance(unitSpawnSettingses).AsSingle();
            Container.Bind<IUnitDataIndexResolver>().To<UnitDataIndexResolver>().AsSingle();

            // Prototype
            Container.BindMemoryPool<UnitBehaviour, UnitBehaviour.Pool>().WithInitialSize(10)
                     .FromComponentInNewPrefab(unitPrefab).UnderTransformGroup("UnitPool");

            // Prototype: Bind ITicker and IInitializable to the UnitsSpawner
            Container.BindInterfacesTo<UnitSpawner>().AsSingle();
            
            // Commands installer
            Container.Install<UnitCommandsInstaller>();
        }
    }
}
