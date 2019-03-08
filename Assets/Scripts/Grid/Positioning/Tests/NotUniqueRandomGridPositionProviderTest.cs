using Math.Random;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using Zenject;

namespace Grid.Positioning.Tests {
    [TestFixture]
    internal class NotUniqueRandomGridPositionProviderTest : ZenjectUnitTestFixture {
        [Inject]
        private NotUniqueRandomGridPositionProvider _randomGridPositionProvider;

        private IGrid _grid;
        private IGridPositionCalculator _gridPositionCalculator;
        private AlwaysReturnSameWeightRandomProvider _randomProvider;

        [SetUp]
        public void CommonInstall() {
            _grid = Substitute.For<IGrid>();
            _grid.NumTilesX.Returns((uint)100);
            _grid.NumTilesY.Returns((uint)100);
            _grid.TileSize.Returns((uint)1);
            _gridPositionCalculator = Substitute.For<IGridPositionCalculator>();
            _randomProvider = new AlwaysReturnSameWeightRandomProvider(0);
            
            Container.Bind<IGridPositionCalculator>().To<IGridPositionCalculator>()
                     .FromInstance(_gridPositionCalculator).AsTransient();
            Container.Bind<IRandomProvider>().To<AlwaysReturnSameWeightRandomProvider>().FromInstance(_randomProvider)
                     .AsSingle();
            Container.Bind<NotUniqueRandomGridPositionProvider>().AsSingle();
            Container.Inject(this);
        }
        
        [TestCase(0, 5, 5)]
        [TestCase(5, 2, 7)]
        [TestCase(5, -10, 0)]
        [TestCase(0, 0, 0)]
        public void testGivenRandomPosition_GetsCenterPosition(int center, int selectedDistanceFromCenter, int expectedPosition) {
            Vector2 centerPosition = new Vector2(center, center);
            _randomProvider.SetReturnedWeight(selectedDistanceFromCenter);
            _gridPositionCalculator.GetTileClosestToCenter(_grid)
                                   .Returns(centerPosition);

            Vector2[] positions =
                _randomGridPositionProvider.GetRandomUniquePositions(_grid, selectedDistanceFromCenter, 1);
            Assert.AreEqual(Vector2.one * expectedPosition,
                            positions[0]);
        }
    }
}