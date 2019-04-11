using CommandSystem;
using CommandSystem.Installers;
using Zenject;

namespace Drawing.Commands {
    public class DrawingCommandsInstaller : CommandsInstaller {
        public DrawingCommandsInstaller(CommandFactory commandFactory) : base(commandFactory) { }
        
        protected override void InstallCommandBindings() {
            // These two commands preserve state to be able to Undo().
            // i.e: They keep the state of the pixel before painting it.
            // As such, we want to use transient, so different instances are created in the command queue.
            Container.Bind<ICommand<ClearAllPixelsCommandData>>().To<ClearAllPixelsCommand>().AsTransient();
            Container.Bind<ICommand<PaintPixelData>>().To<PaintPixelCommand>().AsTransient();
        }
    }
}