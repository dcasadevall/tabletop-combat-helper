using CommandSystem;
using CommandSystem.Installers;

namespace Map.Commands {
    public class MapCommandsInstaller : AbstractCommandsInstaller {
        public MapCommandsInstaller(CommandFactory commandFactory) : base(commandFactory) { }
        
        protected override void InstallCommandBindings() {
            Container.Bind<ICommand<LoadMapCommandData>>().To<LoadMapCommand>().AsSingle();
        }
    }
}