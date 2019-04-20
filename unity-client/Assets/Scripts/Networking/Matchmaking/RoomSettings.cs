using UnityEngine;

namespace Networking.Matchmaking {
    public class RoomSettings : ScriptableObject, IRoomSettings {
        public string name;
        public string Name {
            get {
                return name;
            }
        }

        public byte numPlayers;
        public byte NumPlayers {
            get {
                return numPlayers;
            }
        }

        public bool isVisible;
        public bool IsVisible {
            get {
                return isVisible;
            }
        }
    }
}