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
        private readonly IGrid _grid;
        private readonly IRandomProvider _randomProvider;

        public NotUniqueRandomGridPositionProvider(IGrid grid,
                                                   IRandomProvider randomProvider) {
            _grid = grid;
            _randomProvider = randomProvider;
        }

        public IntVector2[] GetRandomUniquePositions(IntVector2 startTile, int maxDistanceFromCenter,
                                                     int numTilesToGenerate) {
            IntVector2[] tiles = new IntVector2[numTilesToGenerate];
            
            for (int i = 0; i < numTilesToGenerate; i++) {
                int xPosition = startTile.x +
                                _randomProvider.GetRandomIntegerInRange(-maxDistanceFromCenter, maxDistanceFromCenter + 1);
                int yPosition = startTile.y +
                                _randomProvider.GetRandomIntegerInRange(-maxDistanceFromCenter, maxDistanceFromCenter + 1);
            
                xPosition = System.Math.Min((int)_grid.NumTilesX - 1, System.Math.Max(0, xPosition));
                yPosition = System.Math.Min((int)_grid.NumTilesY - 1, System.Math.Max(0, yPosition));
                
                tiles[i] = IntVector2.Of(xPosition, yPosition);
            }

            return tiles;
        }
    }
}