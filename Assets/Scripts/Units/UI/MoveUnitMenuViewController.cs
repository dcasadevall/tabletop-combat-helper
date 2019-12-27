using System;
using UniRx.Async;
using UnityEngine;

namespace Units.UI {
    /// <summary>
    /// ViewController used for the Unit Submenu which is shown when unit movement is selected from within
    /// <see cref="UnitMenuViewController"/>
    /// </summary>
    public class MoveUnitMenuViewController : MonoBehaviour {
        public event Action UnitMovementConfirmed;
        public event Action UnitMovementCanceled;

        internal UniTask<bool> Show() {
            return UniTask.FromResult(true);
        }
        
        internal UniTask Hide() {
            return UniTask.CompletedTask;
        }
    }
}