using UnityEngine;

namespace UI.RadialMenu {
    public class RadialMenuViewController : MonoBehaviour, IRadialMenu {
        [SerializeField]
        private Animator _animator;
        
        public void Show() {
            _animator.SetBool("IsOpen", true);
        }

        public void Hide() {
            _animator.SetBool("IsOpen", false);
        }
    }
}