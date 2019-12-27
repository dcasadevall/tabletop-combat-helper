using UnityEngine;

namespace Grid.Highlighting {
    public interface IGridCellHighlightPool {
        IGridCellHighlight Spawn(Vector3 position, Color color);
        void Despawn(IGridCellHighlight gridCellHighlight);
        void DespawnAll();
    }
}