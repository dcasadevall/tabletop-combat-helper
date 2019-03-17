using System.Collections.Generic;
using Math;
using Math.Random;
using UnityEngine;

namespace Grid.Positioning {
    /// <summary>
    /// Implementation of <see cref="IRandomGridPositionProvider"/> which returns a set with no duplicates.
    /// It returns a list of random unique positions walking through the grid from the center outwards in a
    /// spiral pattern, deciding at every step to pick that position or not by a probability range (30%) 
    /// </summary>
    internal class SpiralSequenceRandomPositionProvider : IRandomGridPositionProvider {
        private const float kTileChooseProbability = .3f;
        private const int kMaxTries = 100;
        private readonly IGrid _grid;
        private readonly IRandomProvider _randomProvider;
        private readonly IGridPositionCalculator _gridPositionCalculator;

        public SpiralSequenceRandomPositionProvider(IGrid grid,
                                                    IGridPositionCalculator gridPositionCalculator, 
                                                    IRandomProvider randomProvider) {
            _grid = grid;
            _randomProvider = randomProvider;
            _gridPositionCalculator = gridPositionCalculator;
        }
        
        public IntVector2[] GetRandomUniquePositions(int maxDistanceFromCenter, int numTilesToGenerate) {
            IntVector2 spiralCenter = _gridPositionCalculator.GetTileClosestToCenter();
            IntVector2 spiralEndTile = IntVector2.Of(spiralCenter.x - numTilesToGenerate,
                                                     spiralCenter.y - numTilesToGenerate);
            LinkedList<IntVector2> availableTiles = new LinkedList<IntVector2>();
            
            int k = 1;
            IntVector2 currentPosition, stretchStart = spiralCenter;

            do {
                currentPosition = IntVector2.Of(stretchStart.x, stretchStart.y);
                for (int i = stretchStart.y; i > stretchStart.y - k; i--) {
                    currentPosition = IntVector2.Of(currentPosition.x, i);
                    AddTileIfInsideGridBounds(availableTiles, currentPosition);
                }
                
                //If at this point the position is (center.x - maxDistance, center.y - maxDistance) the spiral is
                //complete for maxDistance.
                if (currentPosition == spiralEndTile) {
                    break;
                }

                stretchStart = IntVector2.Of(currentPosition.x, currentPosition.y - 1);

                currentPosition = IntVector2.Of(stretchStart.x, stretchStart.y);
                for (int i = stretchStart.x; i < stretchStart.x + k; i++) {
                    currentPosition = IntVector2.Of(i, currentPosition.y);
                    AddTileIfInsideGridBounds(availableTiles, currentPosition);
                }

                stretchStart = IntVector2.Of(currentPosition.x + 1, currentPosition.y);
                k++;

                currentPosition = IntVector2.Of(stretchStart.x, stretchStart.y);
                for (int i = stretchStart.y; i < stretchStart.y + k; i++) {
                    currentPosition = IntVector2.Of(currentPosition.x, i);
                    AddTileIfInsideGridBounds(availableTiles, currentPosition);
                }

                stretchStart = IntVector2.Of(currentPosition.x, currentPosition.y + 1);

                currentPosition = IntVector2.Of(stretchStart.x, stretchStart.y);
                for (int i = stretchStart.x; i > stretchStart.x - k; i--) {
                    currentPosition = IntVector2.Of(i, currentPosition.y);
                    AddTileIfInsideGridBounds(availableTiles, currentPosition);
                }

                stretchStart = IntVector2.Of(currentPosition.x - 1, currentPosition.y);
                k++;
            } while (true);

            return ChooseRandomPositionsFromAvailableTiles(availableTiles, numTilesToGenerate);
        }

        private void AddTileIfInsideGridBounds(LinkedList<IntVector2> availableTiles, IntVector2 tile) {
            if (_grid.TileBounds().Contains(tile)) {
                availableTiles.AddLast(tile);
            }
        }

        private IntVector2[] ChooseRandomPositionsFromAvailableTiles(LinkedList<IntVector2> availableTiles,
                                                                     int numTilesToGenerate) {
            IntVector2[] tiles = new IntVector2[numTilesToGenerate];
            int chosenTiles = 0;
            float randomNumber;
            
            for (int i = 0; i < kMaxTries; i++) {
                if (chosenTiles == numTilesToGenerate) {
                    break;
                }
                
                foreach (var tile in availableTiles) {
                    randomNumber = _randomProvider.GetRandomFloatInRange(0, 1);
                    if (randomNumber <= kTileChooseProbability) {
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