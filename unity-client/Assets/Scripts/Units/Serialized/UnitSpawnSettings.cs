using Units.Spawning;
using UnityEngine;

namespace Units.Serialized {
    public class UnitSpawnSettings : ScriptableObject, IUnitSpawnSettings {
        [SerializeField]
        private UnitData[] _nonPlayerUnits;
        public IUnitData[] NonPlayerUnits {
            get {
                return _nonPlayerUnits;
            }
        }

        [SerializeField]
        private UnitData[] _playerUnits;
        public IUnitData[] PlayerUnits {
            get {
                return _playerUnits;
            }
        }

        [SerializeField]
        private int _maxInitialUnitSpawnDistanceToCenter;
        public int MaxInitialUnitSpawnDistanceToCenter {
            get {
                return _maxInitialUnitSpawnDistanceToCenter;
            }
        }
    }
}