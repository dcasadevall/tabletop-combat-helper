using UnityEngine;
using Zenject;

namespace UI.RadialMenu {
    public class RadialMenuViewController : MonoBehaviour, IRadialMenu {
        [SerializeField]
        private Animator _animator;
        
        [SerializeField]
        private Canvas _canvas;

        [Inject]
        public void Construct(Camera worldCamera) {
            _canvas.worldCamera = worldCamera;
        }
        
        public void Show() {
            transform.position = Input.mousePosition;
            _animator.SetBool("IsOpen", true);
        }

        public void Hide() {
            _animator.SetBool("IsOpen", false);
        }
    }
}