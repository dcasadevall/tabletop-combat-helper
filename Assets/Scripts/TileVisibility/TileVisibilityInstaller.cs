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
            Container.Bind<MeshFilter>()
                     .FromComponentInNewPrefab(_fogOfWarMeshFilter)
                     .AsSingle()
                     .WhenInjectedInto<FogOfWarMeshFilterColorSetter>();
            Container.BindInterfacesTo<FogOfWarMeshFilterColorSetter>().AsSingle();
            
            // Needed for fog of war mesh.
            Container.Install<MeshUtilsInstaller>();
        }
    }
}