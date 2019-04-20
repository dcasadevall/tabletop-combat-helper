using System.IO;
using UnityEditor;
using UnityEngine;

namespace Utils.Editor {
    /// <summary>
    /// Utility class to aid in the creation of helper ScriptableObject editor menu items.
    /// </summary>
    public static class ScriptableObjectUtil {
        /// <summary>
        //	This makes it easy to create, name and place unique new ScriptableObject asset files.
        /// </summary>
        public static void CreateAsset<T>() where T : ScriptableObject {
            string path = AssetDatabase.GetAssetPath (Selection.activeObject);
            if (path == "") {
                path = "Assets";
            }
            else if (Path.GetExtension (path) != "") {
                path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
            }

            Selection.activeObject = CreateAssetAtPath<T>(path);
        }


        public static T CreateAssetAtPath<T>(string path) where T : ScriptableObject {
            T asset = ScriptableObject.CreateInstance<T> ();
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + typeof(T).Name + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow ();

            return asset;
        }
    }
}
