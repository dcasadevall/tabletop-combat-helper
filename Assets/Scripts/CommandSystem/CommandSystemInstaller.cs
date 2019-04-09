using Zenject;

namespace CommandSystem {
    public class CommandSystemInstaller : MonoInstaller {
        public override void InstallBindings() {
            Container.Bind<ICommandQueue>().To<InstantCommandQueue>().AsSingle();
        }
    }
}