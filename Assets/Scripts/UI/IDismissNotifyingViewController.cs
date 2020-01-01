namespace UI {
    /// <summary>
    /// Implementors of this interface will allow users to present a view controller via <see cref="Show"/>, which will
    /// send an <see cref="ViewControllerDismissed"/> event when dismissed.
    /// </summary>
    public interface IDismissNotifyingViewController {
        event System.Action ViewControllerDismissed;
        
        void Show();
    }
}