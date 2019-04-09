using CommandSystem;
using CommandSystem.Installers;
using Zenject;

namespace Drawing.Commands {
    public class DrawingCommandsInstaller : CommandsInstaller {
        public DrawingCommandsInstaller(CommandFactory commandFactory) : base(commandFactory) { }
        
        protected override void InstallCommandBindings() {
            Container.Bind<ICommand<ClearAllPixelsCommandData>>().To<ClearAllPixelsCommand>().AsSingle();
            Container.Bind<ICommand<PaintPixelData>>().To<PaintPixelCommand>().AsSingle();
        }
    }
}