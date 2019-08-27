using System;
using Networking.Matchmaking;
using Photon.Realtime;
using UniRx;

namespace Networking.Photon.Matchmaking {
    internal interface IPhotonRoomHandler {
        /// <summary>
        /// Event fired when a room has been left. This should only happen on disconnect.
        /// Listeners may want to attempt reconnect.
        /// </summary>
        IObservable<Unit> RoomLeft { get; }
        
        bool IsRoomHost { get; }
        
        /// <summary>
        /// Joins or creates a new room with the <see cref="IRoomSettings"/> in the current context.
        /// </summary>
        /// <returns></returns>
        IObservable<PhotonRoomJoinResult> JoinOrCreateRoom();

        /// <summary>
        /// A stream of actor numbers which receives values each time another player joins the current room we are in.
        /// </summary>
        IObservable<int> PlayedJoinedRoomStream { get; }
    }
}