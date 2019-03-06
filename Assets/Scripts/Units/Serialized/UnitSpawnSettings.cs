using UnityEngine;

namespace Units.Serialized {
    public class UnitSpawnSettings : ScriptableObject, IUnitSpawnSettings {
        [SerializeField]
        private readonly IUnit[] _nonPlayerUnits;
        public IUnit[] NonPlayerUnits {
            get {
                return _nonPlayerUnits;
            }
        }

        [SerializeField]
        private readonly IUnit[] _playerUnits;
        public IUnit[] PlayerUnits {
            get {
                return _playerUnits;
            }
        }
    }
}