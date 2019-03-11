using System.Collections.Generic;
using Units.Serialized;

namespace Units {
    public class Unit : IUnit {
        public UnitId UnitId { get; }
        public IUnitData UnitData { get; }

        private List<IUnit> _petUnits = new List<IUnit>();
        public IUnit[] PetUnits {
            get {
                return _petUnits.ToArray();
            }
        }
        
        public Unit(IUnitData unitData) : this(new UnitId(), unitData) {
        }

        public Unit(UnitId unitId, IUnitData unitData) {
            UnitId = unitId;
            UnitData = unitData;

            foreach (var petData in unitData.Pets) {
                _petUnits.Add(new Unit(petData));
            }
        }
    }
}