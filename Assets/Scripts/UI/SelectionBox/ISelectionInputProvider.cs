using UnityEngine;

namespace UI.SelectionBox {
    /// <summary>
    /// Implementations of this interface will be used to provide information about input used to determine the current
    /// selection box.
    /// </summary>
    public interface ISelectionInputProvider {
        /// <summary>
        /// Position where the selection currently lives. This is updated every frame.
        /// </summary>
        Vector3 CurrentPosition { get; }
    }
}