using CameraSystem;
using Map.Serialized;
using UnityEngine;
using Zenject;

namespace Map.Rendering {
    public class MapRenderingInstaller : MonoInstaller {
        public GameObject mapPrefab;
        public GameObject tilePrefab;
        public GameObject mapCameraPrefab;
        
        public override void InstallBindings() {
            Container.Bind<IInitializable>().To<MapRenderer>().AsSingle();
            Container.Bind<ITileLoader>()
                     .FromIFactory(x => x.To<TileLoaderFactory>().FromSubContainerResolve()
                                         .ByMethod(InstallMapRendererFactory).AsSingle()).AsSingle();
            
            Container.Bind<MapBehaviour>().AsSingle();
            Container.BindFactory<MapBehaviour, MapBehaviour.Factory>().FromComponentInNewPrefab(mapPrefab)
                     .AsSingle();
            Container.BindMemoryPool<TileRendererBehaviour, TileRendererBehaviour.Pool>().WithInitialSize(40)
                     .FromComponentInNewPrefab(tilePrefab).UnderTransformGroup("Tiles");
            
            Container.Bind<ICameraController>().To<PrototypeCameraController>()
                     .FromComponentInNewPrefab(mapCameraPrefab).AsSingle();
        }

        private void InstallMapRendererFactory(DiContainer container) {
            container.Bind<TileLoaderFactory>().AsSingle();
            container.Bind<RandomizedRepeatedTileLoader>().AsSingle();
            container.Bind<SequentialUniqueTileLoader>().AsSingle();
        }
    }
}