using Math;
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
        
        public IntVector2[] GetRandomUniquePositions(IGrid grid, int maxDistanceFromCenter, int numTilesToGenerate) {
            IntVector2[] tiles = new IntVector2[numTilesToGenerate];
            IntVector2 centerPosition = _gridPositionCalculator.GetTileClosestToCenter(grid);
            
            for (int i = 0; i < numTilesToGenerate; i++) {
                int xPosition = centerPosition.x +
                                _randomProvider.GetRandomIntegerInRange(-maxDistanceFromCenter, maxDistanceFromCenter + 1);
                int yPosition = centerPosition.y +
                                _randomProvider.GetRandomIntegerInRange(-maxDistanceFromCenter, maxDistanceFromCenter + 1);
            
                xPosition = System.Math.Min((int)grid.NumTilesX - 1, System.Math.Max(0, xPosition));
                yPosition = System.Math.Min((int)grid.NumTilesY - 1, System.Math.Max(0, yPosition));
                
                tiles[i] = IntVector2.Of(xPosition, yPosition);
            }

            return tiles;
        }
    }
}