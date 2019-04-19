using System;
using Networking.Matchmaking;
using Photon.Realtime;

namespace Networking.Photon.Matchmaking {
    internal interface IPhotonRoomHandler {
        /// <summary>
        /// Joins or creates a new room with the <see cref="IRoomSettings"/> in the current context.
        /// </summary>
        /// <returns></returns>
        IObservable<PhotonRoomJoinResult> JoinOrCreateRoom();
    }
}