using Drawing.UI;
using InputSystem;
using Replays.Playback;
using Replays.Playback.UI;
using UI;
using Units;
using Units.Editing;
using UnityEngine;
using Zenject;

namespace EncounterOverlay {
    public class EncounterOverlayViewController : MonoBehaviour, IEncounterOverlayViewController {
        // For now, just use a "hidden" flag for all states
        [SerializeField]
        private string _replayPlaybackOpenBoolName = "Hidden";

        [SerializeField]
        private string _drawingOpenBoolName = "Hidden";
        
        [SerializeField]
        private string _inputLockBoolName = "InputLock";

        [SerializeField]
        private Animator _animator;

        private IInputLock _inputLock;
        private IDismissNotifyingViewController _replayPlaybackViewController;
        private IDismissNotifyingViewController _drawingViewController;

        [Inject]
        public void Construct(IInputLock inputLock,
                              [Inject(Id = ReplayPlaybackInstaller.REPLAY_OVERLAY_ID)]
                              IDismissNotifyingViewController replayPlaybackViewController,
                              [Inject(Id = DrawingViewControllerInstaller.DRAWING_OVERLAY_ID)]
                              IDismissNotifyingViewController drawingViewController) {
            _inputLock = inputLock;
            _replayPlaybackViewController = replayPlaybackViewController;
            _drawingViewController = drawingViewController;
        }

        private void Awake() {
            if (_inputLock.IsLocked) {
                HandleInputLockAcquired();
            } else {
                HandleInputLockReleased();
            }
            
            _inputLock.InputLockAcquired += HandleInputLockAcquired;
            _inputLock.InputLockReleased += HandleInputLockReleased;
        }

        private void OnDestroy() {
            _inputLock.InputLockAcquired -= HandleInputLockAcquired;
            _inputLock.InputLockReleased -= HandleInputLockReleased;
        }

        private void HandleInputLockReleased() {
            _animator.SetBool(_inputLockBoolName, false);
        }

        private void HandleInputLockAcquired() {
            _animator.SetBool(_inputLockBoolName, true);
        }

        public void HandleReplayPlaybackButtonPressed() {
            _animator.SetBool(_replayPlaybackOpenBoolName, true);
            _replayPlaybackViewController.ViewControllerDismissed += HandleReplayPlaybackViewControllerDismissed;
            _replayPlaybackViewController.Show();
        }

        private void HandleReplayPlaybackViewControllerDismissed() {
            _animator.SetBool(_replayPlaybackOpenBoolName, false);
            _replayPlaybackViewController.ViewControllerDismissed -= HandleReplayPlaybackViewControllerDismissed;
        }

        public void HandleDrawingButtonPressed() {
            _animator.SetBool(_drawingOpenBoolName, true);
            _drawingViewController.ViewControllerDismissed += HandleDrawingViewControllerDismissed;
            _drawingViewController.Show();
        }

        private void HandleDrawingViewControllerDismissed() {
            _animator.SetBool(_drawingOpenBoolName, false);
            _drawingViewController.ViewControllerDismissed -= HandleDrawingViewControllerDismissed;
        }
    }
}