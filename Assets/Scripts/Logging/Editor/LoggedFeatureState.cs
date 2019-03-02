using System;

namespace Logging.Editor {
    /// <summary>
    /// A helper serializable data object used to save the current state of the editor logged feature selections.
    /// </summary>
    [Serializable]
    public class LoggedFeatureState {
        public readonly string name;
        public bool selected;

        public LoggedFeatureState(string name) {
            this.name = name;
        }
    }
}