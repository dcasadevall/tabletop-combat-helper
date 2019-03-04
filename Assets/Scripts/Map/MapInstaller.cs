using Grid;
using UnityEngine;
using Zenject;

namespace Map {
    public class MapInstaller : MonoInstaller {
        public GameObject mapPrefab;
        
        public override void InstallBindings() {
            Container.Bind<ICombatGrid>().To<CombatGrid>().AsSingle();
            
            // Container.BindInterfacesTo<MapLoader>().AsSingle();
            Container.Bind<MapBehaviour>().AsSingle();
            Container.BindFactory<MapBehaviour, MapBehaviour.Factory>().FromComponentInNewPrefab(mapPrefab);
        }
    }
}
