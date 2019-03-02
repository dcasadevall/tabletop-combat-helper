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
    }
}