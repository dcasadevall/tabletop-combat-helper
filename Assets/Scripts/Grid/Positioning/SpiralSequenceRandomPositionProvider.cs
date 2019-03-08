using System.Collections.Generic;
using System.Numerics;
using Math.Random;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace Grid.Positioning
{
    /// <summary>
    /// Implementation of <see cref="IRandomGridPositionProvider"/> which returns a set with no duplicates.
    /// It returns a list of random unique positions walking through the grid from the center outwards in a
    /// spiral pattern, deciding at every step to pick that position or not by a probability range (30%) 
    /// </summary>
    public class SpiralSequenceRandomPositionProvider : IRandomGridPositionProvider
    {
        private IRandomProvider _randomProvider;
        private IGridPositionCalculator _gridPositionCalculator;
        private const float TileChooseProbability = .3f;
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
            Vector2 spiralCenter = _gridPositionCalculator.GetTileClosestToCenter(grid);
            Vector2 spiralEndTile = new Vector2(spiralCenter.x - numTilesToGenerate, 
                                                spiralCenter.y - numTilesToGenerate);
            LinkedList<Vector2> availableTiles = new LinkedList<Vector2>();
            
            Debug.Log("Spiral Center Tile: (" + (int)spiralCenter.x + "," + 
                      (int)spiralCenter.y + ")");

            int k = 1;
            Vector2 currentPosition, stretchStart = spiralCenter;

            do {
                currentPosition = new Vector2(stretchStart.x, stretchStart.y);
                for (int i = (int) stretchStart.y; i > stretchStart.y - k; i--) {
                    currentPosition.y = i;
                    AddIfAvailableTile(availableTiles, grid, currentPosition);
                }
                
                //If at this point the position is (center.x - maxDistance, center.y - maxDistance) the spiral is
                //complete for maxDistance.
                if (currentPosition == spiralEndTile) break;

                stretchStart = new Vector2(currentPosition.x, currentPosition.y - 1);

                currentPosition = new Vector2(stretchStart.x, stretchStart.y);
                for (int i = (int) stretchStart.x; i < stretchStart.x + k; i++) {
                    currentPosition.x = i;
                    AddIfAvailableTile(availableTiles, grid, currentPosition);
                }

                stretchStart = new Vector2(currentPosition.x + 1, currentPosition.y);
                k++;

                currentPosition = new Vector2(stretchStart.x, stretchStart.y);
                for (int i = (int) stretchStart.y; i < stretchStart.y + k; i++) {
                    currentPosition.y = i;
                    AddIfAvailableTile(availableTiles, grid, currentPosition);
                }

                stretchStart = new Vector2(currentPosition.x, currentPosition.y + 1);

                currentPosition = new Vector2(stretchStart.x, stretchStart.y);
                for (int i = (int) stretchStart.x; i > stretchStart.x - k; i--) {
                    currentPosition.x = i;
                    AddIfAvailableTile(availableTiles, grid, currentPosition);
                }

                stretchStart = new Vector2(currentPosition.x - 1, currentPosition.y);
                k++;
            } while (true);

            return ChooseRandomPositionsFromAvailableTiles(availableTiles, numTilesToGenerate);
        }

        private void AddIfAvailableTile(LinkedList<Vector2> availableTiles, IGrid grid, Vector2 tile) {
            if (grid.TileBounds().Contains(tile)) {
                availableTiles.AddLast(tile);
                Debug.Log("Tile (" + tile.x + "," + tile.y + ") Chosen");
            }
        }

        private Vector2[] ChooseRandomPositionsFromAvailableTiles(LinkedList<Vector2> availableTiles, 
                                                                  int numTilesToGenerate) {

            Vector2[] tiles = new Vector2[numTilesToGenerate];
            int chosenTiles = 0;
            float randomNumber;
            
            for (int i = 0; i < MaxTries; i++) {
                if (chosenTiles == numTilesToGenerate) break;
                
                foreach (var tile in availableTiles) {
                    randomNumber = _randomProvider.GetRandomFloatInRange(0, 1);
                    if (randomNumber <= TileChooseProbability) {
                        tiles[chosenTiles] = tile;
                        chosenTiles++;
                        availableTiles.Remove(tile);
                        break;
                    }
                }
            }

            return tiles;
        }

       
    }
}