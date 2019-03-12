using CameraSystem;
using Map.Serialized;
using Map.UI;
using UnityEngine;
using Zenject;

namespace Map {
    public class MapInstaller : MonoInstaller {
        public GameObject mapSelectViewControllerPrefab;
        public GameObject mapPrefab;
        public GameObject mapCameraPrefab;
        public MapData[] mapDatas;
        
        public override void InstallBindings() {
            Container.Bind<IMapSelectViewController>().To<MapSelectViewController>()
                     .FromComponentInNewPrefab(mapSelectViewControllerPrefab).AsSingle();
            Container.Bind<IInitializable>().To<MapSelectionLoader>().AsSingle();
            
            Container.Bind<MapBehaviour>().AsSingle();
            Container.BindFactory<IMapData, MapBehaviour, MapBehaviour.Factory>().FromComponentInNewPrefab(mapPrefab)
                     .AsSingle();
            Container.Bind<ICameraController>().To<PrototypeCameraController>()
                     .FromComponentInNewPrefab(mapCameraPrefab).AsSingle();
            
            foreach (var mapData in mapDatas) {
                Container.Bind<IMapData>().To<MapData>().FromInstance(mapData);
            }
        }
    }
}
