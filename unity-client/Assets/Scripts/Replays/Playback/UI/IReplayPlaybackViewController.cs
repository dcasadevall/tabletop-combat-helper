namespace Replays.Playback.UI {
    public interface IReplayPlaybackViewController {
        event System.Action CancelReplayButtonPressed;
        
        void Show();
        void Hide();
    }
}