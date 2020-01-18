using Math;
using UnityEngine;

namespace Grid.Highlighting {
    public interface IGridCellHighlightPool {
        IGridCellHighlight Spawn(IntVector2 tileCoords, Color color);
        void Despawn(IGridCellHighlight gridCellHighlight);
        void DespawnAll();
    }
}