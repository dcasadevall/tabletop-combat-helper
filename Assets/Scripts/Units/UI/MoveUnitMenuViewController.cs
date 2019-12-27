using System;
using System.Threading;
using UI.RadialMenu;
using UniRx.Async;
using Units.Actions;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Units.UI {
    /// <summary>
    /// ViewController used for the Unit Submenu which is shown when unit movement is selected from within
    /// <see cref="UnitMenuViewController"/>
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

        internal async UniTask<UnitActionPlanResult> Show(Vector3 screenPosition, CancellationToken token) {
            _radialMenu.Show(screenPosition);
            gameObject.SetActive(true);

            UnitActionPlanResult result;
            using (var confirmHandler = _confirmButton.GetAsyncClickEventHandler(token)) 
            using (var cancelHandler = _cancelButton.GetAsyncClickEventHandler(token)) {
                var clickIndex = await UniTask.WhenAny(confirmHandler.OnClickAsync(), cancelHandler.OnClickAsync());
                result = clickIndex == 0 ? UnitActionPlanResult.Confirmed : UnitActionPlanResult.Canceled;
            }

            return result;
        }

        internal UniTask Hide() {
            return _radialMenu.Hide();
        }
    }
}