using System.Collections.Generic;
using CameraSystem;
using UnityEngine;

namespace Grid.Highlighting {
    public class GridCellHighlightPool : IGridCellHighlightPool {
        private readonly List<IGridCellHighlight> _spawnedHighlights;
        private readonly GridCellHighlight.Pool _monoPool;

        public GridCellHighlightPool(GridCellHighlight.Pool monoPool) {
            _monoPool = monoPool;
            _spawnedHighlights = new List<IGridCellHighlight>();
        }

        public IGridCellHighlight Spawn(Vector2 position, Color color) {
            var highlight = _monoPool.Spawn(new Vector3(position.x, 
                                                        position.y, 
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