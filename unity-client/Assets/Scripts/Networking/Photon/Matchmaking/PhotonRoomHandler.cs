using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Logging;
using Networking.Matchmaking;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UniRx.Async;

namespace Networking.Photon.Matchmaking {
    internal class PhotonRoomHandler : IPhotonRoomHandler, IMatchmakingCallbacks, IInRoomCallbacks, IDisposable {
        private readonly IRoomSettings _roomSettings;
        private readonly ILogger _logger;
        private JoinRoomState _joinRoomState;
        
        private Subject<int> _playerJoinedRoomSubject = new Subject<int>();
        public IObservable<int> PlayedJoinedRoomStream {
            get {
                return _playerJoinedRoomSubject.AsObservable();
            }
        }


        public PhotonRoomHandler(IRoomSettings roomSettings, ILogger logger) {
            _roomSettings = roomSettings;
            _logger = logger;
        }

        public void Dispose() {
            PhotonNetwork.RemoveCallbackTarget(this);
            _playerJoinedRoomSubject?.Dispose();
        }

        public bool IsRoomHost {
            get {
                return PhotonNetwork.IsMasterClient;
            }
        }

        public IObservable<PhotonRoomJoinResult> JoinOrCreateRoom() {
            return JoinOrCreateRoomTask().ToObservable();
        }

        private async UniTask<PhotonRoomJoinResult> JoinOrCreateRoomTask() {
            PhotonNetwork.AddCallbackTarget(this);
            
            // Setup room options
            _joinRoomState = new JoinRoomState();
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsVisible = _roomSettings.IsVisible;
            roomOptions.MaxPlayers = _roomSettings.NumPlayers;
            
            // Join room and await
            _logger.Log(LoggedFeature.Network, "Joining room: {0}", roomOptions);
            PhotonNetwork.JoinOrCreateRoom(_roomSettings.Name, roomOptions, TypedLobby.Default);
            await _joinRoomState.ObserveEveryValueChanged(state => state.isFinished)
                                .Where(isFinished => isFinished)
                                .FirstOrDefault()
                                .Timeout(TimeSpan.FromSeconds(15));
            
            if (!_joinRoomState.success) {
                throw new RoomJoinException(string.Format("Error joining room. Code: {0}. Message: {1}",
                                                          _joinRoomState.errorCode,
                                                          _joinRoomState.message));
            }

            _logger.Log(LoggedFeature.Network, "Joined room. Creator: {0}", _joinRoomState.isCreator);
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

        public void OnJoinRandomFailed(short returnCode, string message) {
        }

        public void OnLeftRoom() {
            _logger.LogError(LoggedFeature.Network, "Left room while joining.");
            _joinRoomState.success = false;
        }
        #endregion

        #region IInRoomCallbacks
        public void OnPlayerEnteredRoom(global::Photon.Realtime.Player newPlayer) {
            _logger.Log(LoggedFeature.Network, "New player joined the room: {0}", newPlayer);
            _playerJoinedRoomSubject.OnNext(newPlayer.ActorNumber);
        }

        public void OnPlayerLeftRoom(global::Photon.Realtime.Player otherPlayer) {
        }

        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) {
        }

        public void OnPlayerPropertiesUpdate(global::Photon.Realtime.Player targetPlayer, Hashtable changedProps) {
        }

        public void OnMasterClientSwitched(global::Photon.Realtime.Player newMasterClient) {
        }
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