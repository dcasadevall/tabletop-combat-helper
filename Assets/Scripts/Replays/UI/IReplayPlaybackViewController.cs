namespace Replays.UI {
    public interface IReplayPlaybackViewController {
        event System.Action CancelReplayButtonPressed;
        
        event System.Action PlayButtonPressed;
        event System.Action PauseButtonPressed;
        event System.Action ForwardButtonPressed;
        event System.Action EjectButtonPressed;

        event System.Action<float> ScrubSliderDragged;

        void Show();
        void Hide();
    }
}