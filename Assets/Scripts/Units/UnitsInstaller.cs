using Prototype;
using Units.Serialized;
using Units.UI;
using UnityEngine;
using Zenject;

namespace Units {
    public class UnitsInstaller : MonoInstaller {
        public UnitData[] unitDatas;
        public GameObject unitPickerViewController;
        public GameObject unitPrefab;
        
        public override void InstallBindings() {
            Container.Bind<IUnitPickerViewController>().FromComponentInNewPrefab(unitPickerViewController).AsSingle();
            foreach (var unitData in unitDatas) {
                Container.Bind<IUnitData>().To<UnitData>().FromInstance(unitData);
            }

            // Prototype
            Container.BindMemoryPool<UnitBehaviour, UnitBehaviour.Pool>().WithInitialSize(10)
                     .FromComponentInNewPrefab(unitPrefab).UnderTransformGroup("UnitPool");

            // Prototype: Bind ITicker and IInitializable to the UnitsSpawner
            Container.BindInterfacesTo<UnitSpawner>().AsSingle();
        }
    }
}
