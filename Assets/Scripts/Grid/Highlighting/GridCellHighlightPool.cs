using System.Collections.Generic;
using CameraSystem;
using Grid.Positioning;
using Math;
using UnityEngine;

namespace Grid.Highlighting {
    public class GridCellHighlightPool : IGridCellHighlightPool {
        private readonly List<IGridCellHighlight> _spawnedHighlights;
        private readonly IGridPositionCalculator _gridPositionCalculator;
        private readonly GridCellHighlight.Pool _monoPool;

        public GridCellHighlightPool(IGridPositionCalculator gridPositionCalculator, GridCellHighlight.Pool monoPool) {
            _gridPositionCalculator = gridPositionCalculator;
            _monoPool = monoPool;
            _spawnedHighlights = new List<IGridCellHighlight>();
        }

        public IGridCellHighlight Spawn(IntVector2 tileCoords, Color color) {
            var worldPosition = _gridPositionCalculator.GetTileCenterWorldPosition(tileCoords);
            var highlight = _monoPool.Spawn(new Vector3(worldPosition.x, 
                                                        worldPosition.y, 
                                                        DepthConstants.CELL_HIGHLIGHT_DEPTH),
                                            color);
            _spawnedHighlights.Add(highlight);

            return highlight;
        }

        public void Despawn(IGridCellHighlight gridCellHighlight) {
            _monoPool.Despawn((GridCellHighlight) gridCellHighlight);
            _spawnedHighlights.Remove(gridCellHighlight);
        }

        public void DespawnAll() {
            for (int i = _spawnedHighlights.Count - 1; i >= 0; i--) {
                Despawn(_spawnedHighlights[i]);
            }
        }
    }
}