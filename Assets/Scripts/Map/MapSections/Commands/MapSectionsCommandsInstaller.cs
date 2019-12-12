using CommandSystem;
using CommandSystem.Installers;
using Zenject;

namespace Map.MapSections.Commands {
    public class MapSectionsCommandsInstaller : CommandsInstaller {
        private readonly MapSectionContext _context = new MapSectionContext();
        
        public MapSectionsCommandsInstaller(CommandFactory commandFactory) : base(commandFactory) { }

        public override void InstallBindings() {
            // We must expose the concrete command for the typed creation to work.
            Container.Bind<IMapSectionContext>().To<MapSectionContext>().FromInstance(_context);

            Container.Bind<MapSectionContext>().FromInstance(_context).AsSingle()
                     .WhenInjectedInto<LoadMapSectionCommand>();
            BindCommand<LoadMapSectionCommand>().AsSingle();
        }
    }
}