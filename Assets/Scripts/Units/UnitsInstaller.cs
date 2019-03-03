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

            Container.BindMemoryPool<UnitNetworkBehaviour, UnitNetworkBehaviour.Pool>().WithInitialSize(10)
                     .FromComponentInNewPrefab(unitPrefab).UnderTransformGroup("UnitPool");
        }
    }
}
