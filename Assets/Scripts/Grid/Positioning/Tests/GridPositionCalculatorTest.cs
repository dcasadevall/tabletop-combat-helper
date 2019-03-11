using Math;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

namespace Grid.Positioning.Tests {
    [TestFixture]
    public class GridPositionCalculatorTest {
        [TestCase(4U, 4U, 3.99f, 3.99f, 3, 3)]
        [TestCase(4U, 4U, 0.01f, 0.01f, 0, 0)]
        [TestCase(1U, 1U, 0.99f, 0.99f, 0, 0)]
        [TestCase(1U, 1U, 0.01f, 0.01f, 0, 0)]
        [TestCase(63U, 23U, 5.0001f, 22.5f, 5, 22)]
        [TestCase(2U, 2U, 1.00001f, 1.0001f, 1, 1)]
        [TestCase(2U, 2U, 1.0f, 2.0f, 1, 1)] // 1.0 and 2.0f considered inside the 1 tile
        public void TestGivenTileInGrid_GetTileContainingWorldPosition_ReturnsCoordinate(
            uint numTilesX, uint numTilesY, float x, float y, int expectedCoordX, int expectedCoordY) {
            IGrid grid = Substitute.For<IGrid>();
            grid.NumTilesX.Returns(numTilesX);
            grid.NumTilesY.Returns(numTilesY);
            grid.TileSize.Returns((uint)1);
            grid.OriginWorldPosition.Returns(new Vector2(0, 0));
            
            IGridPositionCalculator gridPositionCalculator = new GridPositionCalculator();
            IntVector2? position = gridPositionCalculator.GetTileContainingWorldPosition(grid, new Vector2(x, y));
            Assert.NotNull(position);
            Assert.AreEqual(IntVector2.Of(expectedCoordX, expectedCoordY), position);
        }
        
        [TestCase(1.0f, 1.0f, 4U, 4U, 3.99f, 3.99f, 2, 2)]
        [TestCase(-1.0f, -2.5f, 4U, 4U, 0.01f, 0.01f, 1, 2)]
        [TestCase(1.25f, 1.50f, 1U, 1U, 1.51f, 1.51f, 0, 0)]
        public void TestGivenTileInGrid_OriginWorldPositionSet_GetTileContainingWorldPosition_ReturnsCoordinate(
            float worldPositionX, float worldPositionY,
            uint numTilesX, uint numTilesY, float x, float y, int expectedCoordX, int expectedCoordY) {
            IGrid grid = Substitute.For<IGrid>();
            grid.NumTilesX.Returns(numTilesX);
            grid.NumTilesY.Returns(numTilesY);
            grid.TileSize.Returns((uint)1);
            grid.OriginWorldPosition.Returns(new Vector2(worldPositionX, worldPositionY));
            
            IGridPositionCalculator gridPositionCalculator = new GridPositionCalculator();
            IntVector2? position = gridPositionCalculator.GetTileContainingWorldPosition(grid, new Vector2(x, y));
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
            IGrid grid = Substitute.For<IGrid>();
            grid.NumTilesX.Returns(numTilesX);
            grid.NumTilesY.Returns(numTilesY);
            grid.TileSize.Returns((uint)1);
            
            IGridPositionCalculator gridPositionCalculator = new GridPositionCalculator();
            Assert.IsNull(gridPositionCalculator.GetTileContainingWorldPosition(grid, new Vector2(x, y)));
        }
    }
}