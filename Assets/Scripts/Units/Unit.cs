using System.Collections.Generic;
using Units.Serialized;
using UnityEngine;

namespace Units {
    public class Unit : IUnit {
        public UnitId UnitId { get; }

        private List<IUnit> _petUnits = new List<IUnit>();
        public IUnit[] PetUnits {
            get {
                return _petUnits.ToArray();
            }
        }

        public Unit(UnitId unitId, IUnitData unitData) {
            UnitId = unitId;
            
            foreach (var data in unitData.Pets) {
                _petUnits.Add(new Unit(new UnitId(), data));
            }
        }
    }
}