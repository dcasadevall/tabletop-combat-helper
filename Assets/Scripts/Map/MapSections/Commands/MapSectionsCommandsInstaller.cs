using CommandSystem;
using CommandSystem.Installers;
using Zenject;

namespace Map.MapSections.Commands {
    public class MapSectionsCommandsInstaller : Installer {
        private readonly SerialCommandQueue _serialCommandQueue;
        private readonly MapSectionContext _context = new MapSectionContext();

        public MapSectionsCommandsInstaller(SerialCommandQueue serialCommandQueue) {
            _serialCommandQueue = serialCommandQueue;
        }

        public override void InstallBindings() {
            // We must expose the concrete command for the typed creation to work.
            Container.Bind<IMapSectionContext>().To<MapSectionContext>().FromInstance(_context);
            Container.Bind<MapSectionContext>()
                     .FromInstance(_context)
                     .AsSingle()
                     .WhenInjectedInto<LoadMapSectionCommand>();

            // SerialCommandQueue is injected only to this installer.
            Container.Bind<IPausableCommandQueue>()
                     .To<SerialCommandQueue>()
                     .FromInstance(_serialCommandQueue)
                     .AsSingle()
                     .WhenInjectedInto<LoadMapSectionCommand>();
            Container.Bind<LoadMapSectionCommand>().AsSingle();
        }
    }
}