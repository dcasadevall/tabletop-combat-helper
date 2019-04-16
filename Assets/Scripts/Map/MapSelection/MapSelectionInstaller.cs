using Map.Commands;
using Map.Serialized;
using Map.UI;
using UnityEngine;
using Zenject;

namespace Map.MapSelection {
    public class MapSelectionInstaller : MonoInstaller {
        public GameObject mapSelectViewControllerPrefab;
        public MapData[] mapDatas;
        
        public override void InstallBindings() {
            Container.Bind<IMapSelectViewController>().To<MapSelectViewController>()
                     .FromComponentInNewPrefab(mapSelectViewControllerPrefab).AsSingle();
            
            foreach (var mapData in mapDatas) {
                Container.Bind<IMapData>().To<MapData>().FromInstance(mapData);
            }
            
            Container.Install<MapCommandsInstaller>();
        }
    }
}
