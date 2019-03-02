#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Logging {
    /// <summary>
    /// An Editor only class that allows for toggling logging for different features based on editor preferences.
    /// </summary>
    public static class LoggingConfig {
        private const string kConfigPrefix = "DebugConfig_";
        
        public static bool ShouldLogFeature(LoggedFeature feature) {
#if UNITY_EDITOR
            // By default, log everything but todos.
            if (!EditorPrefs.HasKey(kConfigPrefix + feature)) {
                return feature != LoggedFeature.TODO;
            }
            
            return EditorPrefs.GetBool(kConfigPrefix + feature);
#else
            return feature != LoggedFeature.TODO;
#endif
        }

        public static void SetShouldLogFeature(LoggedFeature feature, bool shouldLog) {
#if UNITY_EDITOR
            EditorPrefs.SetBool(kConfigPrefix + feature, shouldLog);
#endif
        }
    }
}