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
            Container.Bind(typeof(IGridUnitManager), typeof(IInitializable)).To<GridUnitManager>().AsSingle();
            Container.Bind<IRandomGridPositionProvider>().To<SpiralSequenceRandomPositionProvider>().AsSingle();
            Container.Bind<IGridPositionCalculator>().To<GridPositionCalculator>().AsSingle();
            Container.BindInterfacesTo<GridInputManager>().AsSingle();
            
            // GridUnitManager should be the only thing mutating the unit transform.
            Container.Bind<IUnitTransformRegistry>().To<UnitRegistry>().FromResolve().WhenInjectedInto<GridUnitManager>();

            // Grid Visualization
            Container.Bind<IGridCellHighlightPool>().To<GridCellHighlightPool>().AsSingle();
            Container.BindMemoryPool<GridCellHighlight, GridCellHighlight.Pool>().WithInitialSize(10)
                     .FromComponentInNewPrefab(gridCellPrefab).UnderTransformGroup("CellHighlights")
                     .WhenInjectedInto<GridCellHighlightPool>();
#if DEBUG
            Container.BindInterfacesTo<GridVisualizer>().AsSingle();
#endif
            
            CommandsInstaller.Install<GridCommandsInstaller>(Container);
        }
    }
}