using System;
using System.Threading.Tasks;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Async {
    public static class ButtonExtensions {
        /// <summary>
        /// Creates a task from the given button click event, which will be canceled gracefully if the
        /// <see cref="cancelButton"/> is pressed.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="cancelButton"></param>
        /// <returns></returns>
        public static Task<CancellableTaskResult<Unit>>
            ToButtonCancellableTask(this Button button, Button cancelButton) {
            // Note: Button.OnClickAsObservable seems to emit multiple events (does not unregister properly)
            IObservable<Unit> observable = Observable.FromEvent(action => button.onClick.AddListener(action.Invoke),
                                                                action => button.onClick.RemoveListener(action.Invoke));
            return observable.ToButtonCancellableTask(cancelButton);
        }
    }
}