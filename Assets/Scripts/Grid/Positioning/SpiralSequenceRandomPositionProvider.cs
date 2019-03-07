using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Math.Random;
using UnityEngine;
using Grid;
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

        private IGridCoordinates _centerPosition;
        private int _maxDistanceFromCenter;

        private IGridCoordinates _leftTopCorner;
        private IGridCoordinates _rightTopCorner;
        private IGridCoordinates _leftBottomCorner;
        private IGridCoordinates _rightBottomCorner;
        
        public SpiralSequenceRandomPositionProvider(IGridPositionCalculator gridPositionCalculator, 
            IRandomProvider randomProvider)
        {
            _randomProvider = randomProvider;
            _gridPositionCalculator = gridPositionCalculator;
        }
        
        //TODO: Change gridPositionCalculator and GetRandomUniquePositions to return IGridCoordinates.
        public Vector2[] GetRandomUniquePositions(IGrid grid, int maxDistanceFromCenter, int numTilesToGenerate)
        {
            Vector2[] tiles;
            
            Vector2 centerPosition = _gridPositionCalculator.GetTileClosestToCenter(grid);
            _centerPosition.X = (int) centerPosition.x;
            _centerPosition.Y = (int) centerPosition.y;
            
            _maxDistanceFromCenter = maxDistanceFromCenter;
            
            LinkedList<Vector2> availablePositions = new LinkedList<Vector2>();
            
            Debug.Log("Grid Center Tile: (" + (int)_centerPosition.X + "," + 
                      (int)_centerPosition.Y + ")");

            IGridCoordinates currentPosition = _centerPosition;
            _leftTopCorner = _rightTopCorner = _leftBottomCorner = _rightBottomCorner = _centerPosition;
            
            availablePositions.AddLast(new Vector2(_centerPosition.X, _centerPosition.Y));

            while (!IsSpiralCompleted(grid))
            {
                _leftBottomCorner.X = currentPosition.X;
                _leftBottomCorner.Y = currentPosition.Y;
                
                for (int i = (int) _leftTopCorner.Y - 1; i < _rightBottomCorner.Y; i--)
                {
                    currentPosition.Y = i;
                    if (IsCellInBounds(currentPosition, grid))
                    {
                        _leftBottomCorner.Y = i;
                    }
                    else
                    {
                        currentPosition.Y += 1;
                        break;
                    }
                }

                if (IsSpiralCompleted(grid)) break;
                

                //Debug.Log("Tile (" + xPosition + "," + yPosition + ") Chosen");
            }

            tiles = availablePositions.Take(numTilesToGenerate).ToArray();
            return tiles;
            
        }

        private bool IsCellInBounds(IGridCoordinates cell, IGrid grid)
        {
            if (cell.X < 0 || cell.X >= grid.NumTilesX || cell.Y < 0 || cell.Y >= grid.NumTilesY)
            {
                return false;
            }

            if (cell.X < _centerPosition.X - _maxDistanceFromCenter || cell.X > _centerPosition.X + _maxDistanceFromCenter)
            {
                return false;
            }

            if (cell.Y < _centerPosition.Y - _maxDistanceFromCenter || cell.Y > _centerPosition.Y + _maxDistanceFromCenter)
            {
                return false;
            }

            return true;
        }

        private bool IsSpiralCompleted(IGrid grid)
        {
            if ((_leftTopCorner.X == 0 || 
                 _leftTopCorner.X == _centerPosition.X - _maxDistanceFromCenter) &&
                (_leftTopCorner.Y == grid.NumTilesY - 1 ||
                 _leftTopCorner.Y == _centerPosition.Y + _maxDistanceFromCenter))
            {
                if ((_leftBottomCorner.X == 0 ||
                     _leftBottomCorner.X == _centerPosition.X - _maxDistanceFromCenter) &&
                    (_leftBottomCorner.Y == 0 || 
                     _leftBottomCorner.Y == _centerPosition.Y - _maxDistanceFromCenter))
                {
                    if ((_rightTopCorner.X == grid.NumTilesX - 1 ||
                         _rightTopCorner.X == _centerPosition.X + _maxDistanceFromCenter) &&
                        (_rightTopCorner.Y == grid.NumTilesY - 1 ||
                         _leftTopCorner.Y == _centerPosition.Y + _maxDistanceFromCenter))
                    {
                        if ((_rightBottomCorner.X == grid.NumTilesX - 1 ||
                             _leftBottomCorner.X == _centerPosition.X - _maxDistanceFromCenter) &&
                            (_rightBottomCorner.Y == 0 ||
                             _rightBottomCorner.Y == _centerPosition.Y - _maxDistanceFromCenter))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        
    }
}