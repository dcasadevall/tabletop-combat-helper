using Math.Random;
using UnityEngine;

namespace Grid.Positioning {
    /// <summary>
    /// Implementation of <see cref="IRandomGridPositionProvider"/> which returns a set with possible duplicates.
    /// It simply generates a number from -maxDistanceFromCenter to maxDistanceFromCenter for both the x and y
    /// position.
    /// </summary>
    public class NotUniqueRandomGridPositionProvider : IRandomGridPositionProvider {
        private IRandomProvider _randomProvider;
        private IGridPositionCalculator _gridPositionCalculator;

        public NotUniqueRandomGridPositionProvider(IGridPositionCalculator gridPositionCalculator,
                                                   IRandomProvider randomProvider) {
            _randomProvider = randomProvider;
            _gridPositionCalculator = gridPositionCalculator;
        }
        
        public Vector2[] GetRandomUniquePositions(IGrid grid, int maxDistanceFromCenter, int numTilesToGenerate) {
            Vector2[] tiles = new Vector2[numTilesToGenerate];
            Vector2 centerPosition = _gridPositionCalculator.GetTileClosestToCenter(grid);
            
            for (int i = 0; i < numTilesToGenerate; i++) {
                int xPosition = System.Math.Max(0,
                                                (int)centerPosition.x -
                                                _randomProvider.GetRandomIntegerInRange(0, maxDistanceFromCenter));
                int yPosition = System.Math.Max(0,
                                                (int)centerPosition.y -
                                                _randomProvider.GetRandomIntegerInRange(0, maxDistanceFromCenter));
                tiles[i] = new Vector2(xPosition, yPosition);
            }

            return tiles;
        }
    }
}