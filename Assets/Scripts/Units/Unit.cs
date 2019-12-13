using System.Collections.Generic;
using Math;
using Units.Serialized;
using UnityEngine;

namespace Units {
    public class Unit : ITransformableUnit, IUnit {
        public UnitId UnitId { get; }
        public IUnitData UnitData { get; }
        public Transform Transform { get; }

        private List<IUnit> _petUnits = new List<IUnit>();
        public IUnit[] PetUnits {
            get {
                return _petUnits.ToArray();
            }
        }

        public TransformData TransformData {
            get {
                return TransformData.Of(Transform);
            }
        }

        public Unit(UnitId unitId, IUnitData unitData, IUnit[] petUnits, Transform transform) {
            UnitId = unitId;
            UnitData = unitData;
            Transform = transform;
            _petUnits.AddRange(petUnits);
        }
    }
}