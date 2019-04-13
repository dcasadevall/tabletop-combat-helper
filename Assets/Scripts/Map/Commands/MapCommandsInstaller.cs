using CommandSystem;
using CommandSystem.Installers;

namespace Map.Commands {
    public class MapCommandsInstaller : AbstractCommandsInstaller {
        public MapCommandsInstaller(CommandFactory commandFactory) : base(commandFactory) { }
        
        protected override void InstallCommandBindings() {
            // We must expose the concrete command for the typed creation to work.
            Container.Bind<LoadMapCommand>().AsSingle();
            Container.Bind<ICommand<LoadMapCommandData>>().To<LoadMapCommand>().FromResolve();
        }
    }
}