using Map.Commands;
using Map.MapSections.Commands;
using Map.Serialized;
using Map.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
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
            
            foreach (var mapData in _mapSelectionData.mapDatas) {
                Container.Bind<IMapData>().To<MapData>().FromInstance(mapData);
            }
            
            // MapCommands directly invoke MapSectionsCommands, so they need to be loaded together.
            Container.Install<MapCommandsInstaller>();
            Container.Install<MapSectionsCommandsInstaller>();
        }
    }
}
