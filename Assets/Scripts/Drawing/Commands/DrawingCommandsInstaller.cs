using CommandSystem;
using CommandSystem.Installers;
using Zenject;

namespace Drawing.Commands {
    public class DrawingCommandsInstaller : Installer {
        public override void InstallBindings() {
            // These two commands preserve state to be able to Undo().
            // i.e: They keep the state of the pixel before painting it.
            // As such, we want to use transient, so different instances are created in the command queue.
            Container.Bind<ClearAllPixelsCommand>().AsTransient();
            Container.Bind<PaintPixelCommand>().AsTransient();
        }
    }
}