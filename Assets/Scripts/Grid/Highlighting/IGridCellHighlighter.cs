using System;
using Math;
using UnityEngine;

namespace Grid.Highlighting {
    /// <summary>
    /// Utility class used to highlight the cell our mouse is hovered on.
    /// 
    /// It gives a less flexible, but simpler to use behaviour than working with <see cref="IGridCellHighlightPool"/>
    /// and <see cref="IGridCellHighlight"/>.
    /// </summary>
    public interface IGridCellHighlighter {
        /// <summary>
        /// Highlights the cell a mouse is hovered on until the returned object is disposed of.
        /// The highlight uses a default red color. We can add a color as argument if needed.
        /// (You can use a 'using' block).
        /// </summary>
        /// <param name="stayHighlighted">If true, the last cell mouse hovered before Dispose() is called will remain highlighted.</param>
        /// <returns></returns>
        IDisposable HighlightCellOnMouseOver(bool stayHighlighted = false);
        /// <summary>
        /// Manually highlight the given tile coordinates. Remains highlighted until <see cref="ClearHighlight"/> is called.
        /// </summary>
        /// <param name="tileCoords"></param>
        void SetHighlight(IntVector2 tileCoords);
        /// <summary>
        /// Clears any highlight that may be left due to calling <see cref="HighlightCellOnMouseOver(bool)"/>
        /// when stayHighlighted is true, or calling <see cref="SetHighlight"/>.
        /// </summary>
        void ClearHighlight();
    }
}