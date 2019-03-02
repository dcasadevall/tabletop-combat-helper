using Grid;
using Ninject.Syntax;
using Ninject.Unity;

namespace Modules {
    public class GridBinder : BinderMono {
        public override void Bind(IBindingRoot binding) {
            binding.Bind<ICombatGrid>().To<CombatGrid>().InSingletonScope();
        }
    }
}
