using System;
using CommandSystem.Installers;
using Grid.Commands;
using Grid.Highlighting;
using Grid.Positioning;
using Units.Spawning;
using UnityEngine;
using Zenject;

namespace Grid {
    internal class GridInstaller : MonoInstaller {
        public GameObject gridCellPrefab;
        private IDisposable _commandsDisposable;
        
        public override void InstallBindings() {
            Container.Bind<IGrid>().To<Grid>().AsSingle();
            Container.Bind<IRandomGridPositionProvider>().To<SpiralSequenceRandomPositionProvider>().AsSingle();
            Container.Bind<IGridPositionCalculator>().To<GridPositionCalculator>().AsSingle();
            Container.BindInterfacesTo<GridInputManager>().AsSingle();
            
            // Grid Visualization
            Container.Bind<IGridCellHighlighter>().To<GridCellHighlighter>().AsSingle();
            Container.Bind<IGridCellHighlightPool>().To<GridCellHighlightPool>().AsSingle();
            Container.BindMemoryPool<GridCellHighlight, GridCellHighlight.Pool>().WithInitialSize(10)
                     .FromComponentInNewPrefab(gridCellPrefab).UnderTransformGroup("CellHighlights")
                     .WhenInjectedInto<GridCellHighlightPool>();
#if DEBUG
            Container.BindInterfacesTo<GridVisualizer>().AsSingle();
#endif
        }
    }
}