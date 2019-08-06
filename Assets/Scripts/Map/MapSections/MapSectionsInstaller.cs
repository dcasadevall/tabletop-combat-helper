using Map.MapSections.Commands;
using Map.MapSections.UI;
using UnityEngine;
using Zenject;

namespace Map.MapSections {
    public class MapSectionsInstaller : MonoInstaller {
        [SerializeField]
        private GameObject _mapSelectionViewController;

        public override void InstallBindings() {
            Container.Bind<MapSectionSelectionViewController>().FromComponentInNewPrefab(_mapSelectionViewController)
                     .AsSingle().NonLazy();
            
            // MapSectionsCommands loaded in MapSelectionInstaller, since loading the map depends on loading a section.
            // TODO: change structure of scenes instead.
        }
    }
}