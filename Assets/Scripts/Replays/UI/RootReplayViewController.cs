using System;
using UnityEngine;
using Zenject;

namespace Replays.UI {
    public class RootReplayViewController : MonoBehaviour, IRootReplayViewController {
        [SerializeField]
        private string _replayPlaybackOpenBoolName = "ReplayPlaybackOpen";
        
        [SerializeField]
        private Animator _animator;

        private IReplayPlaybackViewController _replayPlaybackViewController;

        [Inject]
        public void Construct(IReplayPlaybackViewController replayPlaybackViewController) {
            _replayPlaybackViewController = replayPlaybackViewController;
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
    }
}