using UniRx.Async;
using UnityEngine;

namespace UI.RadialMenu {
    public interface IRadialMenu {
        UniTask Show(Vector3 screenPosition);
        UniTask Hide();
    }
}