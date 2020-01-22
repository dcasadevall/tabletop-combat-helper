using CommandSystem.Installers;
using Map.MapData.Store.Commands;
using MapEditor;
using Zenject;

namespace Map.MapData.Store {
    public class MapDataStoreInstaller : Installer {
        // Map Selection Data is loaded in a preload scene
        // and injected here.
        private readonly MapSelectionData _mapSelectionData;

        public MapDataStoreInstaller(MapSelectionData mapSelectionData) {
            _mapSelectionData = mapSelectionData;
        }

        public override void InstallBindings() {
            for (uint i = 0; i < _mapSelectionData.mapReferences.Length; i++) {
                var mapReference = _mapSelectionData.mapReferences[i];
                Container.Bind<IMapReference>()
                         .To<AddressableAssetMapReference>()
                         .FromInstance(mapReference);
                
                mapReference.MapStoreId = new MapStoreId(i);
                Container.Bind<AddressableAssetMapReference>()
                         .FromInstance(mapReference)
                         .WhenInjectedInto<AddressableAssetMapDataStore>();
            }
            Container.Bind<IMapDataStore>()
                     .To<AddressableAssetMapDataStore>()
                     .AsSingle()
                     .WhenInjectedInto(typeof(LoadMapCommand), typeof(MapEditorInstaller));

            CommandsInstaller.Install<MapDataStoreCommandsInstaller>(Container);
        }
    }
}