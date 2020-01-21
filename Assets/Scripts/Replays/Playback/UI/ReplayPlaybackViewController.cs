using System;
using InputSystem;
using Map;
using Map.MapData;
using Replays.Persistence;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Replays.Playback.UI {
    public class ReplayPlaybackViewController : MonoBehaviour, IDismissNotifyingViewController {
        public event Action ViewControllerDismissed;

        [SerializeField]
        private Slider _scrubSlider;

        [SerializeField]
        private string _isPlayingBoolName = "IsPlaying";

        [SerializeField]
        private Animator _animator;

        private IInputLock _inputLock;
        private IMapData _mapData;
        private IReplayPlaybackManager _playbackManager;
        private ICommandHistorySaver _commandHistorySaver;
        private IDisposable _lockToken;
        private bool _wasPausedBeforeDragging;
        private bool _wasPlayingBeforeDragging;

        [Inject]
        public void Construct(IInputLock inputLock, IMapData mapData, IReplayPlaybackManager playbackManager,
                              ICommandHistorySaver commandHistorySaver) {
            _inputLock = inputLock;
            _mapData = mapData;
            _playbackManager = playbackManager;
            _commandHistorySaver = commandHistorySaver;
            _playbackManager.PlaybackInterrupted += HandlePlaybackInterrupted;
        }

        private void HandlePlaybackInterrupted() {
            HandleCancelReplayButtonPressed();
        }

        private void Start() {
            Hide();
        }

        private void Update() {
            if (_lockToken == null) {
                _scrubSlider.value = _playbackManager.Progress;
            }

            if (_playbackManager.Progress.Equals(1.0f)) {
                _animator.SetBool(_isPlayingBoolName, false);
            }

            _animator.SetBool(_isPlayingBoolName, _playbackManager.IsPlaying && !_playbackManager.IsPaused);
        }

        public void Show() {
            _playbackManager.Seek(1.0f);
            gameObject.SetActive(true);
        }

        public void Hide() {
            gameObject.SetActive(false);
            ViewControllerDismissed?.Invoke();
        }

        public void HandlePlayButtonPressed() {
            _playbackManager.Play();
        }

        public void HandlePauseButtonPressed() {
            _playbackManager.Pause();
        }

        public void HandleForwardButtonPressed() { }

        public void HandleSaveReplayButtonPressed() {
            _commandHistorySaver.SaveCommandHistory(_mapData.MapName);
        }

        public void HandleCancelReplayButtonPressed() {
            _playbackManager.Stop();
            Hide();
        }

        public void HandleSliderDragBegin(BaseEventData baseEventData) {
            if (_inputLock.IsLocked && _lockToken == null) {
                return;
            }

            _lockToken = _inputLock.Lock();
            _wasPausedBeforeDragging = _playbackManager.IsPaused;
            _wasPlayingBeforeDragging = _playbackManager.IsPlaying;
        }

        public void HandleSliderDrag(BaseEventData baseEventData) {
            _playbackManager.Seek(_scrubSlider.value);
            _playbackManager.Pause();
        }

        public void HandleSliderDragEnd(BaseEventData baseEventData) {
            _lockToken?.Dispose();
            _lockToken = null;

            if (!_wasPausedBeforeDragging && _wasPlayingBeforeDragging) {
                _playbackManager.Play();
            }
        }
    }
}