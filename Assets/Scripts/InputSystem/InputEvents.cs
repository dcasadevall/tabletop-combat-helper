using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InputSystem {
    /// <summary>
    /// Helper class with a set of input related observables.
    /// </summary>
    public class InputEvents : IInputEvents {
        public IObservable<Unit> LeftMouseClick { get; }
        public IObservable<Unit> RightMouseClick { get; }

        public InputEvents(EventSystem eventSystem) {
            LeftMouseClick = GetClickObservable(0, _ => !eventSystem.IsPointerOverGameObject());
            RightMouseClick = GetClickObservable(1, _ => !eventSystem.IsPointerOverGameObject());
        }

        private IObservable<Unit> GetClickObservable(int button, params Func<long, bool>[] whereStatements) {
            IObservable<long> mouseDownStream = Observable.EveryUpdate().Where(_ => Input.GetMouseButtonDown(button));
            foreach (var whereStatement in whereStatements) {
                mouseDownStream = mouseDownStream.Where(whereStatement);
            }

            IObservable<long> mouseUpStream = Observable.EveryUpdate().Where(_ => Input.GetMouseButtonUp(button));
            foreach (var whereStatement in whereStatements) {
                mouseUpStream = mouseUpStream.Where(whereStatement);
            }

            return mouseDownStream.Select(pos => mouseUpStream).Switch().AsUnitObservable();
        }
    }
}