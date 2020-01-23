using Drawing.UI;
using InputSystem;
using Replays.Playback;
using Replays.Playback.UI;
using UI;
using Units;
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

        public async void HandleReplayPlaybackButtonPressed() {
            _animator.SetBool(_replayPlaybackOpenBoolName, true);
            await _replayPlaybackViewController.Show();
            _animator.SetBool(_replayPlaybackOpenBoolName, false);
        }

        public async void HandleDrawingButtonPressed() {
            _animator.SetBool(_drawingOpenBoolName, true);
            await _drawingViewController.Show();
            _animator.SetBool(_drawingOpenBoolName, false);
        }
    }
}