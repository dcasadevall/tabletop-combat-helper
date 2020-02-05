using UniRx.Async;

namespace UI {
    /// <summary>
    /// Implementors of this interface will allow users to present a view controller via <see cref="Show"/>, which will
    /// return an async task completed once it has been dismissed.
    /// </summary>
    public interface IDismissNotifyingViewController {
        UniTask Show();
    }
}