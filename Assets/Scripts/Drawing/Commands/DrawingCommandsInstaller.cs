using CommandSystem;
using CommandSystem.Installers;
using Zenject;

namespace Drawing.Commands {
    public class DrawingCommandsInstaller : CommandsInstaller {
        public DrawingCommandsInstaller(CommandFactory commandFactory) : base(commandFactory) { }
        
        public override void InstallBindings() {
            // These two commands preserve state to be able to Undo().
            // i.e: They keep the state of the pixel before painting it.
            // As such, we want to use transient, so different instances are created in the command queue.
            BindCommand<ClearAllPixelsCommand>().AsTransient();
            BindCommand<PaintPixelCommand>().AsTransient();
        }
    }
}