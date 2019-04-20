using System;

namespace Replays.Persistence {
    [Serializable]
    public class PersistenceLayerSettings {
        public string savePath = "Replays";
        public string duplicateFormat = "{0}_{1}";
    }
}