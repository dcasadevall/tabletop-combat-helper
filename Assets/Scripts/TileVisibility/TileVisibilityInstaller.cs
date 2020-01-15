using TileVisibility.FogOfWar;
using UnityEngine;
using Utils.Mesh;
using Zenject;

namespace TileVisibility {
    public class TileVisibilityInstaller : MonoInstaller {
        [SerializeField]
        private MeshFilter _fogOfWarMeshFilter;
        
        public override void InstallBindings() {
            Container.BindInterfacesTo<TileVisibilityUpdater>().AsSingle();
            Container.Bind<FogOfWarMeshFilterColorSetter>()
                     .AsSingle()
                     .WhenInjectedInto<FogOfWarMeshTileVisibilityDelegate>();
            Container.Bind<MeshFilter>()
                     .FromComponentInNewPrefab(_fogOfWarMeshFilter)
                     .AsSingle()
                     .WhenInjectedInto<FogOfWarMeshTileVisibilityDelegate>();
            Container.BindInterfacesTo<FogOfWarMeshTileVisibilityDelegate>().AsSingle();
            
            // Needed for fog of war mesh.
            Container.Install<MeshUtilsInstaller>();
        }
    }
}