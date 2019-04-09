using System;
using System.Runtime.Serialization;

namespace Units {
    /// <summary>
    /// Id used to globally identifier a <see cref="Units.IUnit"/>.
    /// It should be unique across the application.
    /// </summary>
    [Serializable]
    public class UnitId : ISerializable {
        private readonly Guid _guid;

        public UnitId() {
            _guid = Guid.NewGuid();
        }

        public UnitId(Guid guid) {
            _guid = guid;
        }
        
        #region ISerializable
        public UnitId(SerializationInfo info, StreamingContext context) {
            _guid = new Guid(info.GetString("guid"));
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("guid", _guid.ToString());
        }
        #endregion

        #region Object overrides
        public override bool Equals(Object obj) {
            if (!(obj is UnitId)) {
                return false;
            }

            UnitId otherUnitId = (UnitId)obj;
            return _guid.Equals(otherUnitId._guid);
        }      
   
        public override int GetHashCode() {
            return _guid.GetHashCode();
        }
   
        public override string ToString() {
            return _guid.ToString();
        }
        #endregion

        #region Operators
        public static implicit operator string(UnitId unitId) {
            return unitId.ToString();
        }
        
        public static bool operator ==(UnitId lhs, UnitId rhs) {
            if (ReferenceEquals(lhs, null)) {
                return rhs == null;
            }

            if (ReferenceEquals(rhs, null)) {
                return false;
            }
            
            return lhs._guid.Equals(rhs._guid);
        }

        public static bool operator !=(UnitId lhs, UnitId rhs) {
            return !(lhs == rhs);
        }
        #endregion
    }
}