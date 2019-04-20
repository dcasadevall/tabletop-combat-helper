namespace Networking.Photon.Matchmaking {
    internal class PhotonRoomJoinResult {
        public readonly bool isFirstParticipant;
        
        public PhotonRoomJoinResult(bool isFirstParticipant) {
            this.isFirstParticipant = isFirstParticipant;
        }
    }
}