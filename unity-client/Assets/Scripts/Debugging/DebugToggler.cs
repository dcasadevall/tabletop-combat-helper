using UnityEngine;
using Zenject;

namespace Debugging {
    internal class DebugToggler : ITickable {
        private SerializableDebugSettings _serializableDebugSettings;
        
        public DebugToggler(SerializableDebugSettings debugSettings) {
            _serializableDebugSettings = debugSettings;
        }
        
        public void Tick() {
            if (Input.GetKeyUp(KeyCode.G)) {
                _serializableDebugSettings.showDebugGrid = !_serializableDebugSettings.showDebugGrid;
            }
        }
    }
}