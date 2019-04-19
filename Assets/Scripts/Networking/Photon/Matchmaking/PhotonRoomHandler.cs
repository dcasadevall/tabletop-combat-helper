using System;
using System.Collections.Generic;
using Networking.Matchmaking;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UniRx.Async;
using Zenject;

namespace Networking.Photon.Matchmaking {
    internal class PhotonRoomHandler : IPhotonRoomHandler, IInitializable, IDisposable, IMatchmakingCallbacks {
        private readonly IRoomSettings _roomSettings;
        private JoinRoomState _joinRoomState;

        public PhotonRoomHandler(IRoomSettings roomSettings) {
            _roomSettings = roomSettings;
        }

        public void Initialize() {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public void Dispose() {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public IObservable<PhotonRoomJoinResult> JoinOrCreateRoom() {
            return JoinOrCreateRoomTask().ToObservable();
        }

        private async UniTask<PhotonRoomJoinResult> JoinOrCreateRoomTask() {
            _joinRoomState = new JoinRoomState();
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsVisible = _roomSettings.IsVisible;
            roomOptions.MaxPlayers = _roomSettings.NumPlayers;
            PhotonNetwork.JoinOrCreateRoom(_roomSettings.Name, roomOptions, TypedLobby.Default);
            await Observable.EveryUpdate().Where(_ => _joinRoomState.isFinished).Timeout(TimeSpan.FromSeconds(5));

            if (!_joinRoomState.success) {
                throw new RoomJoinException(string.Format("Error joining room. Code: {0}. Message: {1}",
                                                          _joinRoomState.errorCode,
                                                          _joinRoomState.message));
            }

            return new PhotonRoomJoinResult(_joinRoomState.isCreator);
        }

        #region IMatchMakingCallbacks
        public void OnFriendListUpdate(List<FriendInfo> friendList) { }

        public void OnCreatedRoom() {
            _joinRoomState.isCreator = true;
        }

        public void OnCreateRoomFailed(short returnCode, string message) {
            _joinRoomState.errorCode = returnCode;
            _joinRoomState.message = message;
            _joinRoomState.isFinished = true;
        }

        public void OnJoinedRoom() {
            // JoinOrCreateRoom will trigger both OnJoinedRoom and OnCreatedRoom, so we can use this event
            // as the means to know if we have finished joining the room in both cases.
            _joinRoomState.isFinished = true;
            _joinRoomState.success = true;
        }

        public void OnJoinRoomFailed(short returnCode, string message) {
            _joinRoomState.errorCode = returnCode;
            _joinRoomState.message = message;
            _joinRoomState.isFinished = true;
        }

        public void OnJoinRandomFailed(short returnCode, string message) { }

        public void OnLeftRoom() { }

        #endregion

        private class JoinRoomState {
            public bool success;
            public bool isFinished;
            public bool isCreator;
            public short errorCode;
            public string message;
        }
    }
}