using CommandSystem;
using CommandSystem.Installers;
using Zenject;

namespace Drawing.Commands {
    public class DrawingCommandsInstaller : AbstractCommandsInstaller {
        public DrawingCommandsInstaller(ICommandBinder commandBinder) : base(commandBinder) { }

        public override void InstallBindings() {
            // These two commands preserve state to be able to Undo().
            // i.e: They keep the state of the pixel before painting it.
            // As such, we want to use transient, so different instances are created in the command queue.
            BindCommand<ClearAllPixelsCommand>(binder => binder.AsTransient());
            BindCommand<PaintPixelCommand>(binder => binder.AsTransient());
        }
    }
}