using System;

namespace Logging {
    /// <summary>
    /// Implementors will provide a logging mechanism to be used when log methods are called.
    /// This helps abstract any underlying log system from consumers.
    /// </summary>
    public interface ILogger {
        /// <summary>
        /// Logs the given format with the given tokens, using the same syntax as <see cref="format"/>.
        /// Allows filtering by <see cref="LoggedFeature"/> to disable certain features for logging.
        /// May differentiate different <see cref="LoggedFeature"/>s via log text color.
        /// </summary>
        /// <param name="loggedFeature"></param>
        /// <param name="format"></param>
        /// <param name="tokens"></param>
        void Log(LoggedFeature loggedFeature, String format, params string[] tokens);
        
        /// <summary>
        /// Logs an error for the given feature, with the given format string and tokens.
        /// Syntax used is the same as <see cref="format"/>.
        /// </summary>
        /// <param name="loggedFeature"></param>
        /// <param name="format"></param>
        /// <param name="tokens"></param>
        void LogError(LoggedFeature loggedFeature, String format, params string[] tokens);
    }
}
