using Zenject;

namespace CommandSystem.Installers {
    public class CommandSystemInstaller : MonoInstaller{
        public override void InstallBindings() {
            Container.Bind<CommandFactory>().AsSingle().WhenInjectedInto<CommandFactoryInstaller>();
            Container.Bind<ICommandQueue>().To<InstantCommandQueue>().AsSingle();
            
            Container.Install<CommandFactoryInstaller>();
        }
    }
}