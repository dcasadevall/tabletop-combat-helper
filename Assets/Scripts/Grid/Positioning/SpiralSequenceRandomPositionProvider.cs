using System.Numerics;
using Math.Random;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace Grid.Positioning
{
    /// <summary>
    /// Implementation of <see cref="IRandomGridPositionProvider"/> which returns a set with no duplicates.
    /// It returns a list of random unique positions walking through the grid from the center outwards in a
    /// spiral pattern, deciding at every step to pick that position or not by a probability range (50%) 
    /// </summary>
    public class SpiralSequenceRandomPositionProvider : IRandomGridPositionProvider
    {
        private IRandomProvider _randomProvider;
        private IGridPositionCalculator _gridPositionCalculator;
        private const float CellPickProbabilityRange = .5f;
        private const int MaxTries = 100;

        public SpiralSequenceRandomPositionProvider(IGridPositionCalculator gridPositionCalculator, 
            IRandomProvider randomProvider)
        {
            _randomProvider = randomProvider;
            _gridPositionCalculator = gridPositionCalculator;
        }
        
        public Vector2[] GetRandomUniquePositions(IGrid grid, int maxDistanceFromCenter, int numTilesToGenerate)
        {
            //Test algorithm
            Vector2[] tiles = new Vector2[numTilesToGenerate];
            Vector2 centerPosition = _gridPositionCalculator.GetTileClosestToCenter(grid);
            
            Debug.Log("Grid Center Tile: (" + (int)centerPosition.x + "," + 
                      (int)centerPosition.y + ")");

            for (int i = 0; i < numTilesToGenerate; i++)
            {
                int xPosition = (int)centerPosition.x;
                int yPosition = (int) centerPosition.y + i;
                tiles[i] = new Vector2(xPosition, yPosition);
                
                Debug.Log("Tile (" + xPosition + "," + yPosition + ") Chosen");
            }

            return tiles;
        }
    }
}