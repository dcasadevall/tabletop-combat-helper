using UnityEditor;
using Utils.Editor;

namespace Units.Serialized.Editor {
    /// <summary>
    /// Helper class used to show a menu item to aid creating <see cref="UnitData"/> objects.
    /// </summary>
    public class UnitDataEditor {
        [MenuItem("Assets/Create/UnitData")]
        public static void CreateAsset() {
            ScriptableObjectUtil.CreateAsset<UnitData>();
        }
    }
}