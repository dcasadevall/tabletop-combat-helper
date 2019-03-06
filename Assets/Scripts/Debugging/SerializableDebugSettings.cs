using System;

namespace Debugging {
    [Serializable]
    public class SerializableDebugSettings : IDebugSettings {
        public bool showDebugGrid;
        public bool ShowDebugGrid {
            get {
                return showDebugGrid;
            }
        }
    }
}