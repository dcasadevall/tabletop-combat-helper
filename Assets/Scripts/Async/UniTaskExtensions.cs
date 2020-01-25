using System;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using UniRx.Async;
using UnityEngine.UI;

namespace Async {
    public static class UniTaskExtensions {
        /// <summary>
        /// Creates a task from the given uniTask, which will be canceled gracefully if a value is received from the
        /// given cancellation stream.
        /// </summary>
        /// <param name="uniTask"></param>
        /// <param name="cancelStream"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static async Task<CancellableTaskResult<TResult>> ToCancellableTask<TResult>(
            this UniTask<TResult> uniTask,
            IObservable<Unit> cancelStream) {
            try {
                var cancelSource = new CancellationTokenSource();
                using (cancelStream.Subscribe(_ => cancelSource.Cancel())) {
                    TResult result = await Task.Run(() => uniTask.ToTask(), cancelSource.Token);
                    return CancellableTaskResult<TResult>.FromResult(result);
                }
            } catch (OperationCanceledException e) {
                return CancellableTaskResult<TResult>.FromCancellation();
            }
        }

        /// <summary>
        /// Creates a task from the given uniTask, which will be canceled gracefully if the given button is pressed.
        /// </summary>
        /// <param name="uniTask"></param>
        /// <param name="button"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static Task<CancellableTaskResult<TResult>> ToButtonCancellableTask<TResult>(
            this UniTask<TResult> uniTask, Button button) {
            return uniTask.ToCancellableTask(button.OnClickAsObservable());
        }
        
        /// <summary>
        /// Creates a task from the given uniTask, which will be canceled gracefully if the given button is pressed.
        /// </summary>
        /// <param name="uniTask"></param>
        /// <param name="button"></param>
        /// <returns></returns>
        public static Task<CancellableTaskResult<AsyncUnit>> ToButtonCancellableTask(
            this UniTask uniTask, Button button) {
            return uniTask.AsAsyncUnitUniTask().ToCancellableTask(button.OnClickAsObservable());
        }

        /// <summary>
        /// Creates a Task from the given UniTask.
        /// </summary>
        /// <param name="uniTask"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        private static async Task<TResult> ToTask<TResult>(this UniTask<TResult> uniTask) {
            return await uniTask;
        }
    }
}