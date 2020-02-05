using Math;
using UnityEngine;

namespace Grid.Highlighting {
    /// <summary>
    /// Provides the ability to highlight / clear individual cells.
    /// Gives a more fined grained control than <see cref="IGridCellHighlighter"/>, but it requires more configuration.
    /// </summary>
    public interface IGridCellHighlightPool {
        IGridCellHighlight Spawn(IntVector2 tileCoords, Color color);
        void Despawn(IGridCellHighlight gridCellHighlight);
        void DespawnAll();
    }
}