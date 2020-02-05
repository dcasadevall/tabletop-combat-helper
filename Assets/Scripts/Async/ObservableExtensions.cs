using System;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using UnityEngine.UI;

namespace Async {
    public static class ObservableExtensions {
        /// <summary>
        /// Creates a task from the given observable, which will be canceled gracefully if value is received from the
        /// given cancellation stream.
        /// </summary>
        /// <param name="observable"></param>
        /// <param name="cancelStream"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static async Task<CancellableTaskResult<TResult>> ToCancellableTask<TResult>(
            this IObservable<TResult> observable,
            IObservable<Unit> cancelStream) {
            try {
                var cancelSource = new CancellationTokenSource();
                using (cancelStream.Subscribe(_ => cancelSource.Cancel())) {
                    TResult result =
                        await Task.Run(() => observable.TakeUntil(cancelStream)
                                                       .First()
                                                       .ToTask(cancelSource.Token),
                                       cancelSource.Token);
                    return CancellableTaskResult<TResult>.FromResult(result);
                }
            } catch (OperationCanceledException e) {
                return CancellableTaskResult<TResult>.FromCancellation();
            }
        }

        /// <summary>
        /// Creates a task from the given observable, which will be canceled gracefully if the given button is pressed.
        /// </summary>
        /// <param name="observable"></param>
        /// <param name="button"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static Task<CancellableTaskResult<TResult>> ToButtonCancellableTask<TResult>(
            this IObservable<TResult> observable, Button button) {
            return observable.ToCancellableTask(button.OnClickAsObservable());
        }
    }
}