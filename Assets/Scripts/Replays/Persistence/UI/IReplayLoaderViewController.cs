namespace Replays.Persistence.UI {
    public interface IReplayLoaderViewController {
        event System.Action<string> LoadReplayClicked;

        void Show();
        void Hide();
    }
}