using UnityEngine;

namespace Grid.Highlighting {
    public interface IGridCellHighlightPool {
        IGridCellHighlight Spawn(Vector2 position, Color color);
        void Despawn(IGridCellHighlight gridCellHighlight);
        void DespawnAll();
    }
}