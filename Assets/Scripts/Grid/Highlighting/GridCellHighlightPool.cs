using UnityEngine;

namespace Grid.Highlighting {
    public class GridCellHighlightPool : IGridCellHighlightPool {
        private readonly GridCellHighlight.Pool _monoPool;

        public GridCellHighlightPool(GridCellHighlight.Pool monoPool) {
            _monoPool = monoPool;
        }

        public IGridCellHighlight Spawn(Vector3 position, Color color) {
            return _monoPool.Spawn(position, color);
        }

        public void Despawn(IGridCellHighlight gridCellHighlight) {
            _monoPool.Despawn((GridCellHighlight) gridCellHighlight);
        }
    }
}