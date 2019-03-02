using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Logging.Editor {
    /// <summary>
    /// An editor window that allows developers to select which features should be logged via our <see cref="ILogger"/>.
    /// </summary>
    public class DebugConfigEditorWindow : EditorWindow {
        public List<LoggedFeatureState> featureList;
        
        [MenuItem("Logging/Logging Settings")]
        private static void OpenWindow() {
            DebugConfigEditorWindow window = GetWindow<DebugConfigEditorWindow>();
            
            window.featureList = new List<LoggedFeatureState>();

            foreach (LoggedFeature loggedFeature in GetLoggedFeatures().Values) {
                LoggedFeatureState state = new LoggedFeatureState(loggedFeature.name);
                state.selected = LoggingConfig.ShouldLogFeature(loggedFeature.name);
                window.featureList.Add(state);
            }

            window.Show();
        }
	
        private static Dictionary<string, LoggedFeature> GetLoggedFeatures() {
            return new LoggedFeature().GetType()
                                      .GetProperties(BindingFlags.Public | BindingFlags.Static)
                                      .Where(f => f.PropertyType == typeof(LoggedFeature))
                                      .ToDictionary(f => f.Name,
                                                    f => (LoggedFeature)f.GetValue(null, null));
        }
        
        private void OnGUI() {
            GUILayout.Label("Logged Features", EditorStyles.boldLabel);

            for (int i = 0; i < featureList.Count; i++) {
                bool selected = EditorGUILayout.Toggle(featureList[i].name, featureList[i].selected);
                if (selected != featureList[i].selected) {
                    featureList[i].selected = selected;
                    LoggingConfig.SetShouldLogFeature(new LoggedFeature(featureList[i].name), selected);
                }
            }
        }
    }
}
