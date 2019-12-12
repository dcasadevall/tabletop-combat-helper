using CommandSystem;
using CommandSystem.Installers;
using Zenject;

namespace Map.Commands {
    public class MapCommandsInstaller : CommandsInstaller {
        public MapCommandsInstaller(CommandFactory commandFactory) : base(commandFactory) { }
        
        public override void InstallBindings() {
            BindCommand<LoadMapCommand>().AsSingle();
        }
    }
}