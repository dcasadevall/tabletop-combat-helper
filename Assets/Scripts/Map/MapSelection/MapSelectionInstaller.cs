using CommandSystem.Installers;
using Map.MapData;
using Map.MapData.Store;
using Map.MapSelection.Commands;
using UnityEngine;
using Zenject;

namespace Map.MapSelection {
    public class MapSelectionInstaller : MonoInstaller {
        public GameObject mapSelectViewControllerPrefab;
        
        // Map Selection Data is loaded in a preload scene
        // and injected here.
        private MapSelectionData _mapSelectionData;

        [Inject]
        public void Construct(MapSelectionData mapSelectionData) {
            _mapSelectionData = mapSelectionData;
        }
        
        public override void InstallBindings() {
            Container.Bind<IMapSelectViewController>().To<MapSelectViewController>()
                     .FromComponentInNewPrefab(mapSelectViewControllerPrefab).AsSingle();
            
            foreach (var mapReference in _mapSelectionData.mapReferences) {
                // Most actors just see the map reference.
                Container.Bind<IMapReference>().To<AddressableAssetMapReference>().FromInstance(mapReference);
                // LoadMapCommand needs to actually load the asset.
                Container.Bind<IReadOnlyMapAssetStore>()
                         .To<AddressableAssetMapReference>()
                         .FromInstance(mapReference)
                         .WhenInjectedInto<LoadMapCommand>();
            }

            CommandsInstaller.Install<MapSelectionCommandsInstaller>(Container);
        }
    }
}
