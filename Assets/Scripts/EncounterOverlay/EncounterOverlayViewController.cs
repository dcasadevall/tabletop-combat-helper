using Drawing.UI;
using InputSystem;
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

        private IInputLock _inputLock;
        private IReplayPlaybackViewController _replayPlaybackViewController;
        private IDrawingViewController _drawingViewController;

        [Inject]
        public void Construct(IInputLock inputLock,
                              IReplayPlaybackViewController replayPlaybackViewController,
                              IDrawingViewController drawingViewController) {
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
            gameObject.SetActive(true);
        }

        private void HandleInputLockAcquired() {
            gameObject.SetActive(false);
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