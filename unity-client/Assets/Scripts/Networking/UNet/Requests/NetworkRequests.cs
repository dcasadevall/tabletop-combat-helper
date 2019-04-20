using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

namespace Networking.UNet.Requests {
    /// <summary>
    /// A class containing wrapper methods for the Unity Networking requests, using the <see cref="UniRx"/> <see cref="IObservable{T}"/>.
    /// These methods will be called with certain default parameters, adequate to our Server flow.
    /// </summary>
    internal class NetworkRequests {
        private const string kRoomName = "TableTopRoom";
        private const string kPassword = "ThePasswordIsTaco";

        public static IObservable<List<MatchInfoSnapshot>> ListMatches(NetworkMatch networkMatch) {
            Func<NetworkMatch.DataResponseDelegate<List<MatchInfoSnapshot>>, Coroutine> coroutineWrapper =
                delegate(NetworkMatch.DataResponseDelegate<List<MatchInfoSnapshot>> callback) {
                    return networkMatch.ListMatches(startPageNumber: 0,
                                                    resultPageSize: 1,
                                                    matchNameFilter: "",
                                                    filterOutPrivateMatchesFromResults: false,
                                                    eloScoreTarget: 0,
                                                    requestDomain: 0,
                                                    callback: callback);
                };

            return NetworkRequestCoroutineAdapter.AdaptCoroutine(coroutineWrapper);
        }

        public static IObservable<MatchInfo> CreateMatch(NetworkMatch networkMatch, uint matchSize) {
            Func<NetworkMatch.DataResponseDelegate<MatchInfo>, Coroutine> coroutineWrapper =
                delegate(NetworkMatch.DataResponseDelegate<MatchInfo> callback) {
                    return networkMatch.CreateMatch(kRoomName, matchSize, true, kPassword, "", "", 0, 0, callback);
                };

            return NetworkRequestCoroutineAdapter.AdaptCoroutine(coroutineWrapper);
        }

        public static IObservable<MatchInfo> JoinMatch(NetworkMatch networkMatch,
                                                       MatchInfoSnapshot matchInfo) {
            Func<NetworkMatch.DataResponseDelegate<MatchInfo>, Coroutine> coroutineWrapper =
                delegate(NetworkMatch.DataResponseDelegate<MatchInfo> callback) {
                    return networkMatch.JoinMatch(matchInfo.networkId, kPassword, "", "", 0, 0, callback);
                };

            return NetworkRequestCoroutineAdapter.AdaptCoroutine(coroutineWrapper);
        }

        public static IObservable<Unit> DestroyMatch(NetworkMatch networkMatch,
                                                     NetworkID matchNetworkId) {
            Func<NetworkMatch.BasicResponseDelegate, Coroutine> coroutineWrapper =
                delegate(NetworkMatch.BasicResponseDelegate callback) {
                    return networkMatch.DestroyMatch(matchNetworkId, 0, callback);
                };

            return NetworkRequestCoroutineAdapter.AdaptBasicResponseCoroutine(coroutineWrapper);
        }
    }
}