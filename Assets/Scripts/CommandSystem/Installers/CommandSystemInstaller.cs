using Zenject;

namespace CommandSystem.Installers {
    public class CommandSystemInstaller : MonoInstaller {
        public override void InstallBindings() {
            Container.Bind<CommandFactory>().AsSingle().WhenInjectedInto<CommandFactoryInstaller>();
            Container.Bind(typeof(ITickable), typeof(ICommandQueue)).To<SerialCommandQueue>().AsSingle();
//            // TODO: Consider decoupling this so not everyone can access it?
//            // How would specific commands get access to it.
//            Container.Bind(typeof(IPausableCommandQueue)).To<SerialCommandQueue>().AsSingle();
            
            Container.Install<CommandFactoryInstaller>();
        }
    }
}