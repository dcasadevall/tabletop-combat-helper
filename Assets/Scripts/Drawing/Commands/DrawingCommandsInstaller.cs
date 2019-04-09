using CommandSystem;
using Zenject;

namespace Drawing.Commands {
    public class DrawingCommandsInstaller : Installer {
        public override void InstallBindings() {
            Container.Bind<ICommand>().WithId(ClearAllPixelsCommand.Id).To<ClearAllPixelsCommand>().AsSingle();
            Container.Bind<ICommand<PaintPixelData>>().To<PaintPixelCommand>().AsSingle();
        }
    }
}