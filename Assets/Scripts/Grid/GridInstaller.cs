using Grid.Positioning;
using Grid.Serialized;
using UnityEngine;
using Zenject;

namespace Grid {
    internal class GridInstaller : MonoInstaller {
        public GameObject gridCellPrefab;
        
        /// <summary>
        /// Default GridData that will be injected if none is loaded using "WhenInjectedInto" via scene load
        /// </summary>
        [InjectOptional]
        public IGridData gridData = new GridData();
        
        public override void InstallBindings() {
            Container.Bind<IGrid>().To<Grid>().AsSingle();
            Container.BindInstance(gridData).WhenInjectedInto<GridDataLoader>();
            Container.Bind<IInitializable>().To<GridDataLoader>().AsSingle();
            Container.Bind(typeof(IGridUnitManager), typeof(IInitializable)).To<GridUnitManager>().AsSingle();
            Container.Bind<IRandomGridPositionProvider>().To<SpiralSequenceRandomPositionProvider>().AsSingle();
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