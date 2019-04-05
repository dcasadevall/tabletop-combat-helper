using System;
using System.Collections.Generic;

namespace Logging {
    /// <summary>
    /// A class defining a feature to be logged with <see cref="ILogger"/>.
    /// </summary>
    [Serializable]
    public struct LoggedFeature {
        #region Registry
        public static LoggedFeature TODO => new LoggedFeature("TODO");
        public static LoggedFeature Editor => new LoggedFeature("Editor");
        public static LoggedFeature Pooling => new LoggedFeature("Pooling");
        public static LoggedFeature Coroutines => new LoggedFeature("Coroutines");
        public static LoggedFeature Units => new LoggedFeature("Units");
        public static LoggedFeature Map => new LoggedFeature("Map");
        public static LoggedFeature Network => new LoggedFeature("Network");
        public static LoggedFeature Grid => new LoggedFeature("Grid");
        public static LoggedFeature Drawing => new LoggedFeature("Drawing");
        
        private static readonly HashSet<LoggedFeature> _features = new HashSet<LoggedFeature>();
        public static IEnumerable<LoggedFeature> LoggedFeatures => _features;

        private static void Register(LoggedFeature feature) {
            _features.Add(feature);
        }
        #endregion
        
        public readonly string name;

        public LoggedFeature(string name) {
            this.name = name;
            
            Register(this);
        }
        
        #region Equality Methods
        public override bool Equals(object obj) {
            if (obj == null) {
                return false;
            }

            LoggedFeature other = (LoggedFeature)obj;
            return other.name.Equals(name);
        }

        public override int GetHashCode() {
            return name.GetHashCode();
        }
        #endregion

        #region string methods
        public override string ToString() {
            return name;
        }
        #endregion

        #region Operators
        // This allows casting to and from string
        public static implicit operator string(LoggedFeature loggedFeature) {
            return loggedFeature.name;
        }

        public static implicit operator LoggedFeature(string name) {
            return new LoggedFeature(name);
        }

        public static bool operator ==(LoggedFeature a, LoggedFeature b) {
            return a.name.Equals(b.name);
        }

        public static bool operator !=(LoggedFeature a, LoggedFeature b) {
            return !a.name.Equals(b.name);
        }
        #endregion
    }

}