using Math;
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

        private IGridPositionCalculator _gridPositionCalculator;
        private AlwaysReturnSameWeightRandomProvider _randomProvider;

        [SetUp]
        public void CommonInstall() {
            IGrid grid = Substitute.For<IGrid>();
            grid.NumTilesX.Returns((uint)100);
            grid.NumTilesY.Returns((uint)100);
            grid.TileSize.Returns((uint)1);
            _gridPositionCalculator = Substitute.For<IGridPositionCalculator>();
            _randomProvider = new AlwaysReturnSameWeightRandomProvider(0);

            Container.Bind<IGrid>().FromInstance(grid);
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
            IntVector2 centerPosition = IntVector2.Of(center, center);
            _randomProvider.SetReturnedWeight(selectedDistanceFromCenter);

            IntVector2[] positions =
                _randomGridPositionProvider.GetRandomUniquePositions(centerPosition, selectedDistanceFromCenter, 1);
            Assert.AreEqual(IntVector2.One * expectedPosition,
                            positions[0]);
        }
    }
}