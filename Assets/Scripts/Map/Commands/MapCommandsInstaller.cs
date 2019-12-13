using CommandSystem;
using CommandSystem.Installers;
using Zenject;

namespace Map.Commands {
    public class MapCommandsInstaller : AbstractCommandsInstaller {
        public MapCommandsInstaller(ICommandBinder commandBinder) : base(commandBinder) { }

        public override void InstallBindings() {
            BindCommand<LoadMapCommand>(binder => binder.AsSingle());
        }
    }
}