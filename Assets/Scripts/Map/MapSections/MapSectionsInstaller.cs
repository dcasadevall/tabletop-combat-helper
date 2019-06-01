using Map.MapSections.UI;
using UnityEngine;
using Zenject;

namespace Map.MapSections {
    public class MapSectionsInstaller : MonoInstaller {
        [SerializeField]
        private GameObject _mapSelectionViewController;

        private MapSectionContext _context = new MapSectionContext();

        public override void InstallBindings() {
            Container.Bind<IMapSectionContext>().To<MapSectionContext>().FromInstance(_context);
            Container.Bind<MapSectionSelectionViewController>().FromSubContainerResolve().ByMethod(BindViewController)
                     .AsSingle().NonLazy();
        }

        private void BindViewController(DiContainer container) {
            container.Bind<MapSectionContext>().FromInstance(_context);
            container.Bind<MapSectionSelectionViewController>().FromComponentInNewPrefab(_mapSelectionViewController)
                     .AsSingle();
        }
    }
}