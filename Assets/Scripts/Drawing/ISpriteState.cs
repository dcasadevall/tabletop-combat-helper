using UnityEngine;

namespace Drawing {
    /// <summary>
    /// A stateful class that allows us to capture the current state of the given sprite.
    /// Calling <see cref="RestoreState"/> allows us to restore such state.
    /// </summary>
    public interface ISpriteState {
        void RestoreState(Sprite sprite);
    }
}