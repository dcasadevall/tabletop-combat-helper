using UnityEditor;
using UnityEngine;


namespace Utils.Editor {
    public static class CreateScriptableObjectMenuItem {
        [MenuItem("Assets/Create/Scriptable Object")]
        public static void CreateAsset() {
            ScriptableObjectUtil.CreateAsset<ScriptableObject>();
        }
    }
}
