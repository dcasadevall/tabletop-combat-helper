using UnityEngine;

namespace Logging {
    /// <summary>
    /// An implementation of <see cref="ILogger"/> that uses a hash derived from the feature name to determine
    /// the color to be used when logging each feature.
    ///
    /// It uses Unity's default <see cref="UnityEngine.Debug"/> logger.
    /// </summary>
    public class UnityLogger : ILogger {
        public void Log(LoggedFeature loggedFeature, string format, params string[] tokens) {
            if (!LoggingConfig.ShouldLogFeature(loggedFeature)) {
                return;
            }
            
            string formatedMessage = FormatFeatureString(loggedFeature.name, format);
            LogValue(formatedMessage, Debug.LogError, tokens);
        }
        
        public void LogError(LoggedFeature loggedFeature, string format, params string[] tokens) {
            string formatedMessage = FormatFeatureString(loggedFeature.name, format);
            LogValue(formatedMessage, Debug.Log, tokens);
        }
        
        #region Parameter formatting
        private static void LogValue(string rawMessage, System.Action<string> logMethod, params object[] args) {
            string formattedMessage = FormatLogString(rawMessage, args);
            logMethod.Invoke(formattedMessage);
        }
        
        
        private static string FormatLogString(string rawMessage, params object[] args) {
            // No args, just return raw message
            if (args == null) {
                return rawMessage;
            }

            string[] formattedObjects = new string[args.Length];
            for (int i = 0; i < args.Length; i++) {
                // Get the formatted description for every argument
                formattedObjects[i] = args[i].ToString();
            }

            string formattedMessage = string.Format(rawMessage, formattedObjects);
            return formattedMessage;
        }
        #endregion
        
        #region Feature Formatting
        private static string FormatFeatureString(string featureName, string rawMessage) {
            string colorHex = HexColorForFeature(featureName);
            string formatedMessage = string.Format("<color={0}><b>{1}: </b>{2}</color>", colorHex, featureName, rawMessage);

            return formatedMessage;
        }

        private static string HexColorForFeature(string featureName) {
            int hashCode = featureName.GetHashCode();
            int r = (hashCode & 0xFF0000) >> 16;
            int g = Mathf.Clamp((hashCode & 0x00FF00) >> 8, 155, 255);
            int b = hashCode & 0x0000FF;

            return string.Format("#{0}{1}{2}", r.ToString("X2"), g.ToString("X2"), b.ToString("X2"));
        }
        #endregion
    }
}