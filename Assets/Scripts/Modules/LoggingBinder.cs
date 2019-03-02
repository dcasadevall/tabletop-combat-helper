using Logging;
using Ninject.Syntax;
using Ninject.Unity;
using ILogger = Logging.ILogger;

namespace Modules {
    public class LoggingBinder : BinderMono {
        public override void Bind(IBindingRoot binding) {
            binding.Bind<ILogger>().To<UnityLogger>().InSingletonScope();
        }
    }
}
