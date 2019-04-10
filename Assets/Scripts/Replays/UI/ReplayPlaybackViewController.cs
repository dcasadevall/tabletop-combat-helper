using System;
using InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Replays.UI {
    public class ReplayPlaybackViewController : MonoBehaviour, IReplayPlaybackViewController {
        public event Action CancelReplayButtonPressed = delegate {};

        public event Action PlayButtonPressed = delegate {};
        public event Action PauseButtonPressed = delegate {};
        public event Action ForwardButtonPressed = delegate {};
        public event Action EjectButtonPressed = delegate {};
        
        public event Action<float> ScrubSliderDragged = delegate {};
        
        [SerializeField]
        private Slider _scrubSlider;

        [SerializeField]
        private string _isPlayingBoolName = "IsPlaying";

        [SerializeField]
        private Animator _animator;

        private IInputLock _inputLock;
        private Guid? _lockId;

        [Inject]
        public void Construct(IInputLock inputLock) {
            _inputLock = inputLock;
        }

        private void Start() {
            Hide();
        }
        
        public void Show() {
            gameObject.SetActive(true);
        }

        public void Hide() {
            gameObject.SetActive(false);
        }

        public void HandlePlayButtonPressed() {
            _animator.SetBool(_isPlayingBoolName, true);
            PlayButtonPressed.Invoke();
        }
        
        public void HandlePauseButtonPressed() {
            _animator.SetBool(_isPlayingBoolName, false);
            PauseButtonPressed.Invoke();
        }

        public void HandleForwardButtonPressed() {
            ForwardButtonPressed.Invoke();
        }

        public void HandleEjectButtonPressed() {
            EjectButtonPressed.Invoke();
        }
        
        public void HandleCancelReplayButtonPressed() {
            CancelReplayButtonPressed.Invoke();
        }

        public void HandleSliderDragBegin(BaseEventData pointerEventData) {
            if (_inputLock.IsLocked && _lockId == null) {
                return;
            }

            _lockId = _inputLock.Lock();
        }

        public void HandleSliderDragEnd(BaseEventData pointerEventData) {
            if (_lockId != null) {
                _inputLock.Unlock(_lockId.Value);
            }
            
            ScrubSliderDragged.Invoke(_scrubSlider.value);
        }
    }
}