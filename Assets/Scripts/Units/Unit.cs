using System.Collections.Generic;
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

        private Transform _transform;
        
        public Unit(UnitId unitId, IUnit[] pets, Transform transform) {
            UnitId = unitId;
            _petUnits.AddRange(pets);
            _transform = transform;
        }
    }
}