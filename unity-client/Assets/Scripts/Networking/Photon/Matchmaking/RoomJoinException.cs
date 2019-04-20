using System;

namespace Networking.Photon.Matchmaking {
    public class RoomJoinException : Exception {
        public RoomJoinException() : base() {
        }
        
        public RoomJoinException(string message) : base(message) {
        }
    }
}