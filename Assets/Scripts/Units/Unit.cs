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
        
        public Unit(UnitId unitId, IUnitData unitData, IUnit[] petUnits) {
            UnitId = unitId;
            UnitData = unitData;
            _petUnits.AddRange(petUnits);
        }
    }
}