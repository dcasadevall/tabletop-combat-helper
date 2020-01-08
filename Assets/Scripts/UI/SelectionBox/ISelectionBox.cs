using System;
using UniRx.Async;
using UnityEngine;

namespace UI.SelectionBox {
    public interface ISelectionBox {
        /// <summary>
        /// Shows a Selection Box in the game UI while the left mouse button or tap is pressed, and hides it once it's up.
        /// Returns an observable with the World Space Rect that is contained within the selection box once the
        /// left mouse button up or stop tap event is received.
        /// </summary>
        /// <returns>The Rect covered by the SelectionBox, in World Space</returns>
        IObservable<Rect> Show();
        /// <summary>
        /// Stops detecting selections via mouse / taps.
        /// </summary>
        void Hide();
    }
}