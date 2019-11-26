using CommandSystem;
using CommandSystem.Installers;
using Zenject;

namespace Drawing.Commands {
    public class DrawingCommandsInstaller : AbstractCommandsInstaller {
        protected override void InstallCommandBindings() {
            // These two commands preserve state to be able to Undo().
            // i.e: They keep the state of the pixel before painting it.
            // As such, we want to use transient, so different instances are created in the command queue.
            // Note: We must expose the concrete command for the typed creation to work.
            Container.Bind<ClearAllPixelsCommand>().AsTransient();
            Container.Bind<PaintPixelCommand>().AsTransient();
        }
    }
}