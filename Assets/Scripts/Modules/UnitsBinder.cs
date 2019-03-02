using Ninject.Syntax;
using Ninject.Unity;
using Ninject.Unity.Provider;
using Units.Serialized;
using Units.UI;

namespace Modules {
    public class UnitsBinder : BinderMono {
        public UnitData[] unitDatas;
        
        public override void Bind(IBindingRoot binding) {
            binding.Bind<IUnitPickerViewController>().ToProvider<PrefabProvider<UnitPickerViewController>>().InSingletonScope();
            foreach (var unitData in unitDatas) {
                binding.Bind<IUnitData>().ToConstant(unitData);
            }
        }
    }
}
