using System.Threading;
using UI.RadialMenu;
using UniRx.Async;
using Units.Selection;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Units.Movement {
    /// <summary>
    /// ViewController used for the Unit Submenu which is shown when unit movement is selected from within
    /// <see cref="UnitMenuViewController"/>.
    /// 
    /// THIS IS CURRENTLY UNUSED.
    /// </summary>
    public class MoveUnitMenuViewController : MonoBehaviour {
        [SerializeField]
        private Button _confirmButton;

        [SerializeField]
        private Button _cancelButton;

        private IRadialMenu _radialMenu;
        
        [Inject]
        public void Construct() {
            // TODO: Can we inject this instead?
            _radialMenu = GetComponent<IRadialMenu>();
        }

        internal async UniTask<bool> Show(Vector3 screenPosition, CancellationToken token) {
            _radialMenu.Show(screenPosition);
            gameObject.SetActive(true);

            bool didConfirm;
            using (var confirmHandler = _confirmButton.GetAsyncClickEventHandler(token)) 
            using (var cancelHandler = _cancelButton.GetAsyncClickEventHandler(token)) {
                var clickIndex = await UniTask.WhenAny(confirmHandler.OnClickAsync(), cancelHandler.OnClickAsync());
                didConfirm = clickIndex == 0;
            }

            return didConfirm;
        }

        internal UniTask Hide() {
            return _radialMenu.Hide();
        }
    }
}