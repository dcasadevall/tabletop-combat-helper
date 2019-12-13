using Map.MapSections.Commands;
using Zenject;

namespace CommandSystem.Installers {
    public class CommandSystemInstaller : MonoInstaller {
        public override void InstallBindings() {
            Container.Bind<ICommandFactory>().To<CommandFactory>().AsSingle();
            
            // TODO: Uncomment and remove lines below once we can use subcontainer with kernel.
            // Container.Bind<ICommandQueue>().FromSubContainerResolve().ByMethod(BindCommandQueue).AsSingle();
            Container.Bind<SerialCommandQueue>().AsSingle();
            Container.Bind(typeof(IPausableCommandQueue)).To<SerialCommandQueue>().FromResolve();
            Container.Bind(typeof(ITickable), typeof(ICommandQueue)).To<SerialCommandQueue>().FromResolve();
        }

        // Currently, WithKernel is bugged and does not process tickable.
        // TODO: Uncomment and use subcontainer once fixed.
//        private void BindCommandQueue(DiContainer container) {
//            container.Bind<SerialCommandQueue>().AsSingle();
//            container.Bind<SerialCommandQueue>().FromResolve().WhenInjectedInto<MapSectionsCommandsInstaller>();
//            
//            container.Bind(typeof(IPausableCommandQueue)).To<SerialCommandQueue>().FromResolve();
//            container.Bind(typeof(ITickable), typeof(ICommandQueue)).To<SerialCommandQueue>().FromResolve();
//        }
    }
}