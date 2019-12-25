using System.Collections.Generic;
using UnityEngine;

namespace Grid.Highlighting {
    public class GridCellHighlightPool : IGridCellHighlightPool {
        private readonly List<IGridCellHighlight> _spawnedHighlights;
        private readonly GridCellHighlight.Pool _monoPool;

        public GridCellHighlightPool(GridCellHighlight.Pool monoPool) {
            _monoPool = monoPool;
            _spawnedHighlights = new List<IGridCellHighlight>();
        }

        public IGridCellHighlight Spawn(Vector3 position, Color color) {
            var highlight = _monoPool.Spawn(position, color);
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