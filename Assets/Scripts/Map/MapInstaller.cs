using CameraSystem;
using Map.Rendering;
using Map.Serialized;
using Map.UI;
using UnityEngine;
using Zenject;

namespace Map {
    public class MapInstaller : MonoInstaller {
        public GameObject mapSelectViewControllerPrefab;
        public MapData[] mapDatas;
        
        public override void InstallBindings() {
            Container.Bind<IMapSelectViewController>().To<MapSelectViewController>()
                     .FromComponentInNewPrefab(mapSelectViewControllerPrefab).AsSingle();
            Container.Bind<IInitializable>().To<MapSelectionLoader>().AsSingle();
            
            foreach (var mapData in mapDatas) {
                Container.Bind<IMapData>().To<MapData>().FromInstance(mapData);
            }
        }
    }
}
