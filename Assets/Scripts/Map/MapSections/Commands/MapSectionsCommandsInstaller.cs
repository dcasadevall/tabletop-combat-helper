using CommandSystem;
using CommandSystem.Installers;
using Zenject;

namespace Map.MapSections.Commands {
    public class MapSectionsCommandsInstaller : AbstractCommandsInstaller {
        private MapSectionContext _context = new MapSectionContext();
        
        public MapSectionsCommandsInstaller(CommandFactory commandFactory) : base(commandFactory) { }
        
        protected override void InstallCommandBindings() {
            // We must expose the concrete command for the typed creation to work.
            Container.Bind<IMapSectionContext>().To<MapSectionContext>().FromInstance(_context);
            Container.Bind<LoadMapSectionCommand>().FromSubContainerResolve().ByMethod(BindCommand)
                     .AsSingle();
        }

        private void BindCommand(DiContainer container) {
            container.Bind<MapSectionContext>().FromInstance(_context);
            container.Bind<LoadMapSectionCommand>().AsSingle();
        }
    }
}