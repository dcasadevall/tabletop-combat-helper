using Map.Commands;
using Map.MapSections.Commands;
using Map.Serialized;
using Map.UI;
using UnityEngine;
using Zenject;

namespace Map.MapSelection {
    public class MapSelectionInstaller : MonoInstaller {
        public GameObject mapSelectViewControllerPrefab;
        public MapSelectionData mapSelectionData;
        
        public override void InstallBindings() {
            Container.Bind<IMapSelectViewController>().To<MapSelectViewController>()
                     .FromComponentInNewPrefab(mapSelectViewControllerPrefab).AsSingle();
            
            foreach (var mapData in mapSelectionData.mapDatas) {
                Container.Bind<IMapData>().To<MapData>().FromInstance(mapData);
            }
            
            // MapCommands directly invoke MapSectionsCommands, so they need to be loaded together.
            Container.Install<MapCommandsInstaller>();
            Container.Install<MapSectionsCommandsInstaller>();
        }
    }
}
