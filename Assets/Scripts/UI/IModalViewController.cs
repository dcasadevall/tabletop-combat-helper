using System;

namespace UI {
    /// <summary>
    /// Implementations of this interface will present a "modal" which will block interaction with the game,
    /// while displaying the given text to the player.
    /// </summary>
    public interface IModalViewController {
        void Show(String text);
        void Hide();
    }
}