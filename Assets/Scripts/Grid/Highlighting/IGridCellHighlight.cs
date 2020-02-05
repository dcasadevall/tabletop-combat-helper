using UnityEngine;

namespace Grid.Highlighting {
    /// <summary>
    /// Represents a highlight on an individual cell, which can be changed in color.
    /// </summary>
    public interface IGridCellHighlight {
        void SetColor(Color color);
    }
}