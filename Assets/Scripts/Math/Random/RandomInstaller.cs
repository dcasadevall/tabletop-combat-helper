using Zenject;

namespace Math.Random {
    public class RandomInstaller : MonoInstaller {
        public override void InstallBindings() {
            Container.Bind<IRandomProvider>().To<UniformlyDistributedRandomProvider>().AsSingle();
        }  
    }
}