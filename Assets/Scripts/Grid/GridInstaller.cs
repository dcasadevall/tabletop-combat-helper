using Grid.Commands;
using Grid.Positioning;
using Units.Spawning;
using UnityEngine;
using Zenject;

namespace Grid {
    internal class GridInstaller : MonoInstaller {
        public GameObject gridCellPrefab;
        
        public override void InstallBindings() {
            Container.Bind<IGrid>().To<Grid>().AsSingle();
            Container.Bind(typeof(IGridUnitManager), typeof(IInitializable)).To<GridUnitManager>().AsSingle();
            Container.Bind<IRandomGridPositionProvider>().To<SpiralSequenceRandomPositionProvider>().AsSingle();
            Container.Bind<IGridPositionCalculator>().To<GridPositionCalculator>().AsSingle();
            Container.Bind<IGridInputManager>().To<GridInputManager>().AsSingle();
            
            // GridUnitManager should be the only thing mutating the unit transform.
            Container.Bind<IUnitTransformRegistry>().To<UnitRegistry>().FromResolve().WhenInjectedInto<GridUnitManager>();
            
#if DEBUG
            // ITicker and IInitializer
            Container.BindInterfacesTo<GridVisualizer>().AsSingle();
            Container.BindFactory<SpriteRenderer, GridVisualizer.GridCellFactory>()
                     .FromComponentInNewPrefab(gridCellPrefab)
                     .UnderTransformGroup("Cells");
#endif
            
            Container.Install<GridCommandsInstaller>();
        } 
    }
}