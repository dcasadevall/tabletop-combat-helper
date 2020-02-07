using System;

namespace Map.MapData.Store {
    [Serializable]
    public class MapStoreId {
        public readonly uint index;

        public MapStoreId(uint index) {
            this.index = index;
        }

        public override string ToString() {
            return $"{index}";
        }

        public override bool Equals(object other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            if (other.GetType() != GetType()) {
                return false;
            }

            MapStoreId otherMapStoreId = (MapStoreId)other;
            return otherMapStoreId.index == index;
        }

        public override int GetHashCode() {
            return (int) index;
        }
        
        public static bool operator ==(MapStoreId lhs, MapStoreId rhs) {
            return lhs.index == rhs.index && lhs.index == rhs.index;
        }

        public static bool operator !=(MapStoreId lhs, MapStoreId rhs) {
            return !(lhs == rhs);
        }
    }
}