using Zenject;

namespace Logging {
    public class LoggingInstaller : MonoInstaller {
        public override void InstallBindings() {
            Container.Bind<ILogger>().To<UnityLogger>().AsSingle();
        }
    }
}
