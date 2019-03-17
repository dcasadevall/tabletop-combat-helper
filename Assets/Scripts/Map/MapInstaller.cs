using CameraSystem;
using Map.Rendering;
using Map.Serialized;
using Map.UI;
using UnityEngine;
using Zenject;

namespace Map {
    public class MapInstaller : MonoInstaller {
        public GameObject mapSelectViewControllerPrefab;
        public GameObject mapPrefab;
        public GameObject tilePrefab;
        public GameObject mapCameraPrefab;
        public MapData[] mapDatas;
        
        public override void InstallBindings() {
            Container.Bind<IMapSelectViewController>().To<MapSelectViewController>()
                     .FromComponentInNewPrefab(mapSelectViewControllerPrefab).AsSingle();
            Container.Bind<IInitializable>().To<MapSelectionLoader>().AsSingle();
            
            Container.Bind<MapBehaviour>().AsSingle();
            Container.BindFactory<IMapData, MapBehaviour, MapBehaviour.Factory>().FromComponentInNewPrefab(mapPrefab)
                     .AsSingle();
            Container.BindMemoryPool<TileRendererBehaviour, TileRendererBehaviour.Pool>().WithInitialSize(40)
                     .FromComponentInNewPrefab(tilePrefab).UnderTransformGroup("Tiles");
            
            Container.Bind<ICameraController>().To<PrototypeCameraController>()
                     .FromComponentInNewPrefab(mapCameraPrefab).AsSingle();

            Container.Bind<IMapRenderer>()
                     .FromIFactory(x => x.To<MapRendererFactory>().FromSubContainerResolve()
                                         .ByMethod(InstallMapRendererFactory).AsSingle()).AsSingle();
            
            foreach (var mapData in mapDatas) {
                Container.Bind<IMapData>().To<MapData>().FromInstance(mapData);
            }
        }

        private void InstallMapRendererFactory(DiContainer container) {
            container.Bind<MapRendererFactory>().AsSingle();
            container.Bind<RandomizedRepeatedTilesMapRenderer>().AsSingle();
            container.Bind<SequentialUniqueTilesMapRenderer>().AsSingle();
        }
    }
}
