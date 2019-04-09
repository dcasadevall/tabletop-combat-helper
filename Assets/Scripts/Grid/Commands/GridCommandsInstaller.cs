using CommandSystem;
using CommandSystem.Installers;
using Zenject;

namespace Grid.Commands {
    public class GridCommandsInstaller : CommandsInstaller {
        public GridCommandsInstaller(CommandFactory commandFactory) : base(commandFactory) { }
        
        protected override void InstallCommandBindings() {
            Container.Bind<ICommand<MoveUnitData>>().To<MoveUnitCommand>().AsSingle();
        }
    }
}