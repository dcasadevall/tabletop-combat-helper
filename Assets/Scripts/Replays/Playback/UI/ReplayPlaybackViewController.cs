using System;
using InputSystem;
using Map;
using Map.MapData;
using Replays.Persistence;
using UI;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace Replays.Playback.UI {
    public class ReplayPlaybackViewController : MonoBehaviour, IDismissNotifyingViewController {
        [SerializeField]
        private string _isPlayingBoolName = "IsPlaying";

        [SerializeField]
        private Animator _animator;
        
        [SerializeField]
        private Slider _scrubSlider;

        [SerializeField]
        private Button _cancelReplayButton;

        private IInputLock _inputLock;
        private IMapData _mapData;
        private IReplayPlaybackManager _playbackManager;
        private ICommandHistorySaver _commandHistorySaver;
        private IDisposable _lockToken;
        private bool _wasPausedBeforeDragging;
        private bool _wasPlayingBeforeDragging;
        private bool _playbackCanceled;

        [Inject]
        public void Construct(IInputLock inputLock, IMapData mapData, IReplayPlaybackManager playbackManager,
                              ICommandHistorySaver commandHistorySaver) {
            _inputLock = inputLock;
            _mapData = mapData;
            _playbackManager = playbackManager;
            _commandHistorySaver = commandHistorySaver;
            
            Preconditions.CheckNotNull(_animator, _scrubSlider, _cancelReplayButton);
        }

        private void Awake() {
            // Start hidden by default.
            gameObject.SetActive(false);
        }

        private void HandlePlaybackInterrupted() {
            _playbackCanceled = true;
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

        public async UniTask Show() {
            _playbackManager.PlaybackInterrupted += HandlePlaybackInterrupted;
            _playbackManager.Seek(1.0f);
            gameObject.SetActive(true);

            var cancelButtonTask = _cancelReplayButton.OnClickAsync();
            var cancelReplayTask = UniTask.WaitUntil(() => _playbackCanceled);

            await UniTask.WhenAny(cancelButtonTask, cancelReplayTask);
            
            _playbackManager.PlaybackInterrupted -= HandlePlaybackInterrupted;
            _playbackManager.Stop();
            gameObject.SetActive(false);
        }

        public void HandlePlayButtonPressed() {
            _playbackManager.Play();
        }

        public void HandlePauseButtonPressed() {
            _playbackManager.Pause();
        }

        public void HandleSaveReplayButtonPressed() {
            _commandHistorySaver.SaveCommandHistory(_mapData.MapName);
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