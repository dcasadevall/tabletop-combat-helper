using UnityEngine;
using Zenject;

namespace Map {
    public class MapInstaller : MonoInstaller {
        public GameObject mapPrefab;
        
        public override void InstallBindings() {
            Container.Bind<MapBehaviour>().AsSingle();
            Container.BindFactory<MapBehaviour, MapBehaviour.Factory>().FromComponentInNewPrefab(mapPrefab);
            Container.Bind<IInitializable>().To<MapLoader>().AsSingle();
        }
    }
}
