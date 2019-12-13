using System;
using CommandSystem.Installers;
using Zenject;

namespace CommandSystem {
    public class CommandSystemInstaller : MonoInstaller {
        public override void InstallBindings() {
            Container.BindInterfacesTo<CommandBinder>()
                     .FromSubContainerResolve()
                     .ByMethod(BindCommandBinder)
                     .WithKernel()
                     .AsSingle();
            Container.Bind<ICommandFactory>().To<CommandFactory>().AsSingle();

            Container.Bind(typeof(ITickable), typeof(ICommandQueue)).To<SerialCommandQueue>().AsSingle();
//            // TODO: Consider decoupling this so not everyone can access it?
//            // How would specific commands get access to it.
//            Container.Bind(typeof(IPausableCommandQueue)).To<SerialCommandQueue>().AsSingle();
        }

        private void BindCommandBinder(DiContainer container) {
            container.Bind<CommandBinder>().AsSingle();
            container.Bind<ITickable>().To<CommandBinder>().FromResolve();
            container.Bind<IDisposable>().To<CommandBinder>().FromResolve();

            container.Bind<ICommandBinder>()
                     .To<CommandBinder>()
                     .FromResolve()
                     .WhenInjectedInto<AbstractCommandsInstaller>();
        }
    }
}