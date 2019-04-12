using CommandSystem;
using CommandSystem.Installers;
using Zenject;

namespace Grid.Commands {
    public class GridCommandsInstaller : AbstractCommandsInstaller {
        public GridCommandsInstaller(CommandFactory commandFactory) : base(commandFactory) { }
        
        protected override void InstallCommandBindings() {
            // We bind this command as transient so it can store state for Undo()
            Container.Bind<MoveUnitCommand>().AsTransient();
        }
    }
}