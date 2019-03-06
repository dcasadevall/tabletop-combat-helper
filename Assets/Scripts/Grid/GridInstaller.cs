using Grid.Positioning;
using Grid.Serialized;
using UnityEngine;
using Zenject;

namespace Grid {
    public class GridInstaller : MonoInstaller {
        public GameObject gridCellPrefab;
        public GridData gridData;
        
        public override void InstallBindings() {
            Container.Bind<IGrid>().To<Grid>().AsSingle();
            Container.Bind<IGridData>().To<GridData>().FromInstance(gridData);
            Container.Bind<IInitializable>().To<GridDataLoader>().AsSingle();
            Container.Bind(typeof(IGridUnitManager), typeof(IInitializable)).To<GridUnitManager>().AsSingle();
            Container.Bind<IRandomGridPositionProvider>().To<NotUniqueRandomGridPositionProvider>().AsSingle();
            Container.Bind<IGridPositionCalculator>().To<GridPositionCalculator>().AsSingle();
            
#if DEBUG
            // ITicker and IInitializer
            Container.BindInterfacesTo<GridVisualizer>().AsSingle();
            Container.BindFactory<SpriteRenderer, GridVisualizer.GridCellFactory>()
                     .FromComponentInNewPrefab(gridCellPrefab)
                     .UnderTransformGroup("Cells");
#endif
        } 
    }
}