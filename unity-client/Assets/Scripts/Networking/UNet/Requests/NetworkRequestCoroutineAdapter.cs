using System;
using System.Collections;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.Networking.Match;

namespace Networking.UNet.Requests {
    /// <summary>
    /// Helper class used to adapt uNet <see cref="Coroutine"/> based requests to <see cref="UniRx"/> <see cref="IObservable{T}"/>
    /// contracts. This makes it easier to work with such interfaces by using chanining, while abstracting the
    /// Unity Coroutine layer underneath.
    ///
    /// TODO: Figure out a cleaner way of doing this using Unirx
    /// </summary>
    internal class NetworkRequestCoroutineAdapter {
        public static IObservable<TValue> AdaptCoroutine<TValue>(
            Func<NetworkMatch.DataResponseDelegate<TValue>, Coroutine> callableCoroutine) {
            return Observable.FromCoroutine<TValue>((observer, cancellationToken) =>
                                                        AdaptCoroutineInternal(callableCoroutine,
                                                                               observer,
                                                                               cancellationToken));
        }

        private static IEnumerator AdaptCoroutineInternal<TValue>(
            Func<NetworkMatch.DataResponseDelegate<TValue>, Coroutine> callableCoroutine,
            IObserver<TValue> observer,
            CancellationToken cancellationToken) {
            bool requestSuccess = false;
            string errorMsg = "Request Not Called";
            TValue result = default(TValue);
            NetworkMatch.DataResponseDelegate<TValue> wrappedCallback =
                delegate(bool success, string info, TValue value) {
                    requestSuccess = success;
                    errorMsg = info;
                    result = value;
                };

            Coroutine coroutine = callableCoroutine.Invoke(wrappedCallback);
            while (!coroutine.GetAwaiter().IsCompleted && !cancellationToken.IsCancellationRequested) {
                yield return null;
            }

            if (cancellationToken.IsCancellationRequested) {
                yield break;
            }

            if (!requestSuccess) {
                observer.OnError(new Exception(errorMsg));
            } else {
                observer.OnNext(result);
                observer.OnCompleted();
            }
        }

        public static IObservable<Unit> AdaptBasicResponseCoroutine(
            Func<NetworkMatch.BasicResponseDelegate, Coroutine> callableCoroutine) {
            return Observable.FromCoroutine<Unit>((observer, cancellationToken) =>
                                                      AdaptBasicResponseCoroutineInternal(callableCoroutine,
                                                                                          observer,
                                                                                          cancellationToken));
        }
        
        private static IEnumerator AdaptBasicResponseCoroutineInternal(
            Func<NetworkMatch.BasicResponseDelegate, Coroutine> callableCoroutine,
            IObserver<Unit> observer,
            CancellationToken cancellationToken) {
            bool requestSuccess = false;
            string errorMsg = "Request Not Called";
            NetworkMatch.BasicResponseDelegate wrappedCallback =
                delegate(bool success, string info) {
                    requestSuccess = success;
                    errorMsg = info;
                };

            Coroutine coroutine = callableCoroutine.Invoke(wrappedCallback);
            while (!coroutine.GetAwaiter().IsCompleted && !cancellationToken.IsCancellationRequested) {
                yield return null;
            }

            if (cancellationToken.IsCancellationRequested) {
                yield break;
            }

            if (!requestSuccess) {
                observer.OnError(new Exception(errorMsg));
            } else {
                observer.OnNext(Unit.Default);
                observer.OnCompleted();
            }
        }
    }
}