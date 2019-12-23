using UniRx.Async;
using UnityEngine;

namespace UI.RadialMenu {
    public class RadialMenuViewController : MonoBehaviour, IRadialMenu {
        [SerializeField]
        private Animator _animator;
        
        public UniTask Show(Vector3 screenPosition) {
            _animator.transform.position = screenPosition;
            _animator.SetBool("IsOpen", true);
            return UniTask.CompletedTask;
        }

        public UniTask Hide() {
            _animator.SetBool("IsOpen", false);
            return UniTask.CompletedTask;
        }
    }
}