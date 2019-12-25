using System.Collections;
using System.Linq;
using Math;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using Zenject;

namespace Grid.Positioning.Tests {
    [TestFixture]
    public class GridPositionCalculatorTest : ZenjectUnitTestFixture {
        private IGrid _grid;
        private IGridPositionCalculator _positionCalculator;
        
        [SetUp]
        public void CommonInstall() {
            _grid = Substitute.For<IGrid>();
            _grid.TileSize.Returns(1U);

            Container.Bind<IGridPositionCalculator>().To<GridPositionCalculator>().AsSingle();
            Container.Bind<IGrid>().FromInstance(_grid);
            Container.Inject(this);
            
            _positionCalculator = Container.Resolve<IGridPositionCalculator>();
        }
        
        [TestCase(4U, 4U, 3.99f, 3.99f, 3, 3)]
        [TestCase(4U, 4U, 0.01f, 0.01f, 0, 0)]
        [TestCase(1U, 1U, 0.99f, 0.99f, 0, 0)]
        [TestCase(1U, 1U, 0.01f, 0.01f, 0, 0)]
        [TestCase(63U, 23U, 5.0001f, 22.5f, 5, 22)]
        [TestCase(2U, 2U, 1.00001f, 1.0001f, 1, 1)]
        [TestCase(2U, 2U, 1.0f, 2.0f, 1, 1)] // 1.0 and 2.0f considered inside the 1 tile
        public void TestGivenTileInGrid_GetTileContainingWorldPosition_ReturnsCoordinate(
            uint numTilesX, uint numTilesY, float x, float y, int expectedCoordX, int expectedCoordY) {
            _grid.NumTilesX.Returns(numTilesX);
            _grid.NumTilesY.Returns(numTilesY);
            _grid.OriginWorldPosition.Returns(new Vector2(0, 0));
            
            IntVector2? position = _positionCalculator.GetTileContainingWorldPosition(new Vector2(x, y));
            Assert.NotNull(position);
            Assert.AreEqual(IntVector2.Of(expectedCoordX, expectedCoordY), position);
        }
        
        [TestCase(1.0f, 1.0f, 4U, 4U, 3.99f, 3.99f, 2, 2)]
        [TestCase(-1.0f, -2.5f, 4U, 4U, 0.01f, 0.01f, 1, 2)]
        [TestCase(1.25f, 1.50f, 1U, 1U, 1.51f, 1.51f, 0, 0)]
        public void TestGivenTileInGrid_OriginWorldPositionSet_GetTileContainingWorldPosition_ReturnsCoordinate(
            float worldPositionX, float worldPositionY,
            uint numTilesX, uint numTilesY, float x, float y, int expectedCoordX, int expectedCoordY) {
            _grid.NumTilesX.Returns(numTilesX);
            _grid.NumTilesY.Returns(numTilesY);
            _grid.OriginWorldPosition.Returns(new Vector2(worldPositionX, worldPositionY));
            
            IntVector2? position = _positionCalculator.GetTileContainingWorldPosition(new Vector2(x, y));
            Assert.NotNull(position);
            Assert.AreEqual(IntVector2.Of(expectedCoordX, expectedCoordY), position);
        }
        
        
        [TestCase(4U, 4U, 4.01f, 4.01f)]
        [TestCase(4U, 4U, -0.01f, -0.01f)]
        [TestCase(1U, 1U, 1.01f, 1.01f)]
        [TestCase(1U, 1U, -0.01f, -0.01f)]
        [TestCase(63U, 23U, -5.0f, 23.5f)]
        [TestCase(2U, 2U, 2.0001f, 2.00001f)]
        public void TestGivenTileNotInGrid_GetTileContainingWorldPosition_ReturnsNull(uint numTilesX, uint numTilesY, float x, float y) {
            _grid.NumTilesX.Returns(numTilesX);
            _grid.NumTilesY.Returns(numTilesY);
            
            Assert.IsNull(_positionCalculator.GetTileContainingWorldPosition(new Vector2(x, y)));
        }

        [TestCase(1U, 1U, 0, 0)]
        [TestCase(3U, 3U, 1, 1)]
        public void TestGivenOddGridSize_GetTileClosestToCenter_ReturnsCenterTile(uint numTilesX, uint numTilesY, int expectedTileX, int expectedTileY) {
            TestGetTileClosestToCenter(numTilesX, numTilesY, expectedTileX, expectedTileY);
        }

        [TestCase(2U, 2U, 0, 0)]
        [TestCase(4U, 4U, 1, 1)]
        public void TestGivenEvenGridSize_GetTileClosestToCenter_ReturnsLowerBound(uint numTilesX, uint numTilesY, int expectedTileX, int expectedTileY) {
            TestGetTileClosestToCenter(numTilesX, numTilesY, expectedTileX, expectedTileY);
        }

        [TestCase(2U, 1U, 0, 0)]
        [TestCase(3U, 4U, 1, 1)]
        [TestCase(5U, 8U, 2, 3)]
        public void TestMixedGridSize_GetTileClosestToCenter_ReturnsLowerBoundWhenEven(uint numTilesX, uint numTilesY, int expectedTileX, int expectedTileY) {
            TestGetTileClosestToCenter(numTilesX, numTilesY, expectedTileX, expectedTileY);
        }

        private void TestGetTileClosestToCenter(uint numTilesX, uint numTilesY, int expectedTileX, int expectedTileY) {
            _grid.NumTilesX.Returns(numTilesX);
            _grid.NumTilesY.Returns(numTilesY);
            
            IntVector2 tileClosestToCenter = _positionCalculator.GetTileClosestToCenter();
            Assert.AreEqual(IntVector2.Of(expectedTileX, expectedTileY), tileClosestToCenter);  
        }

        private class ValidTestCases : IEnumerable {
            public IEnumerator GetEnumerator() {
                yield return new object[] {
                    1,
                    new[,] {
                        {0, 1, 0},
                        {1, 0, 1},
                        {0, 1, 0}
                    }
                };
                yield return new object[] {
                    2,
                    new[,] {
                        {0, 0, 1, 0, 0},
                        {0, 1, 1, 1, 0},
                        {1, 1, 0, 1, 1},
                        {0, 1, 1, 1, 0},
                        {0, 0, 1, 0, 0}
                    }
                };
                yield return new object[] {
                    3,
                    new[,] {
                        {0, 0, 0, 1, 0, 0, 0},
                        {0, 0, 1, 1, 1, 0, 0},
                        {0, 1, 1, 1, 1, 1, 0},
                        {1, 1, 1, 0, 1, 1, 1},
                        {0, 1, 1, 1, 1, 1, 0},
                        {0, 0, 1, 1, 1, 0, 0},
                        {0, 0, 0, 1, 0, 0, 0},
                    }
                };
            }
        }

        [TestCaseSource(typeof(ValidTestCases))]
        public void TestGivenDistance_GetTilesAtDistance_ReturnsElementsWithinDistance(
            int distance, int[,] resultMatrix) {
            _grid.NumTilesX.Returns(100U);
            _grid.NumTilesY.Returns(100U);

            IntVector2 centerCoords = _positionCalculator.GetTileClosestToCenter();
            var result = _positionCalculator.GetTilesAtDistance(centerCoords, distance);

            for (int y = 0; y < resultMatrix.GetLength(0); y++) {
                for (int x = 0; x < resultMatrix.GetLength(1); x++) {
                    int inResult = resultMatrix[x, y];
                    IntVector2 relativeCoords = IntVector2.Of(x - distance, y - distance);
                    if (inResult == 1) {
                        Assert.IsTrue(result.Contains(centerCoords + relativeCoords));
                    } else {
                        Assert.IsFalse(result.Contains(centerCoords + relativeCoords));
                    }
                }
            }
        }
        
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        public void TestGivenInvalidDistance_GetTilesAtDistance_ReturnsEmptyArray(int distance) {
            _grid.NumTilesX.Returns(100U);
            _grid.NumTilesY.Returns(100U);

            IntVector2 centerCoords = _positionCalculator.GetTileClosestToCenter();
            var result = _positionCalculator.GetTilesAtDistance(centerCoords, distance);
            Assert.IsEmpty(result);
        }
    }
}