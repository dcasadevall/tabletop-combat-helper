using UnityEngine;

namespace Logging.Serialized {
    public class SerializedDebugConfigData : ScriptableObject {
        /// <summary>
        /// These features will be logged by the current <see cref="ILogger"/>
        /// </summarySerializedDebugConfigData>
        public string[] logFeatures;
    }
}