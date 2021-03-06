using CommandSystem.Installers;
using Grid.GridUnits;
using Map.MapSections.Commands;
using Map.MapSections.UI;
using UnityEngine;
using Zenject;

namespace Map.MapSections {
    public class MapSectionsInstaller : MonoInstaller {
        [SerializeField]
        private GameObject _mapSelectionViewController;

        public override void InstallBindings() {
            Container.Bind<MapSectionSelectionViewController>()
                     .FromComponentInNewPrefab(_mapSelectionViewController)
                     .AsSingle()
                     .NonLazy();
            Container.Bind<IMapSectionEntryTileFinder>().To<MapSectionEntryTileFinder>().AsSingle();

            CommandsInstaller.Install<MapSectionsCommandsInstaller>(Container);
        }
    }
}