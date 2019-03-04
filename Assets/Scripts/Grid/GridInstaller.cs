using UnityEngine;
using Zenject;

namespace Grid {
    public class GridInstaller : MonoInstaller {
        public GameObject gridCellPrefab;
        
        public override void InstallBindings() {
            Container.Bind<ICombatGrid>().To<CombatGrid>().AsSingle();
            
#if DEBUG
            // ITicker and IInitializer
            Container.BindInterfacesTo<CombatGridVisualizer>().AsSingle();
            Container.BindFactory<SpriteRenderer, CombatGridVisualizer.GridCellFactory>()
                     .FromComponentInNewPrefab(gridCellPrefab)
                     .UnderTransformGroup("Cells");
#endif
        } 
    }
}