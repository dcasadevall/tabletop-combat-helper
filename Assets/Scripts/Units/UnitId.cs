using System;

namespace Units {
    /// <summary>
    /// Id used to globally identifier a <see cref="Units.IUnit"/>.
    /// It should be unique across the application.
    /// </summary>
    public class UnitId {
        private readonly Guid _guid;

        public UnitId() {
            _guid = Guid.NewGuid();
        }

        public UnitId(Guid guid) {
            _guid = guid;
        }

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

        public static implicit operator string(UnitId unitId) {
            return unitId.ToString();
        }
    }
}