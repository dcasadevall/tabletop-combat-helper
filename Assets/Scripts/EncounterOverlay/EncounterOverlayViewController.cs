using Drawing.UI;
using Replays.Playback.UI;
using UnityEngine;
using Zenject;

namespace EncounterOverlay {
    public class EncounterOverlayViewController : MonoBehaviour, IEncounterOverlayViewController {
        // For now, just use a "hidden" flag for both states
        [SerializeField]
        private string _replayPlaybackOpenBoolName = "Hidden";
        [SerializeField]
        private string _drawingOpenBoolName = "Hidden";
        
        [SerializeField]
        private Animator _animator;

        private IReplayPlaybackViewController _replayPlaybackViewController;
        private IDrawingViewController _drawingViewController;

        [Inject]
        public void Construct(IReplayPlaybackViewController replayPlaybackViewController,
                              IDrawingViewController drawingViewController) {
            _replayPlaybackViewController = replayPlaybackViewController;
            _drawingViewController = drawingViewController;
        }
        
        public void HandleReplayPlaybackButtonPressed() {
            _animator.SetBool(_replayPlaybackOpenBoolName, true);
            _replayPlaybackViewController.CancelReplayButtonPressed += HandleCancelReplayButtonPressed;
            _replayPlaybackViewController.Show();
        }

        private void HandleCancelReplayButtonPressed() {
            _animator.SetBool(_replayPlaybackOpenBoolName, false);
            _replayPlaybackViewController.CancelReplayButtonPressed -= HandleCancelReplayButtonPressed;
            _replayPlaybackViewController.Hide();
        }

        public void HandleDrawingButtonPressed() {
            _animator.SetBool(_drawingOpenBoolName, true);
            _drawingViewController.CancelButtonPressed += HandleCancelDrawingButtonPressed;
            _drawingViewController.Show();
        }

        private void HandleCancelDrawingButtonPressed() {
            _animator.SetBool(_drawingOpenBoolName, false);
            _drawingViewController.CancelButtonPressed -= HandleCancelDrawingButtonPressed;
            _drawingViewController.Hide();
        }
    }
}