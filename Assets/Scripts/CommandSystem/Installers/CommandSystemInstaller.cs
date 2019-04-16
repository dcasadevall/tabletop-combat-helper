using Zenject;

namespace CommandSystem.Installers {
    public class CommandSystemInstaller : MonoInstaller {
        public override void InstallBindings() {
            Container.Bind<CommandFactory>().AsSingle().WhenInjectedInto<CommandFactoryInstaller>();
            Container.Bind(typeof(ITickable), typeof(ICommandQueue)).To<SerialCommandQueue>().AsSingle();
            
            Container.Install<CommandFactoryInstaller>();
        }
    }
}