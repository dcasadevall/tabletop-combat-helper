using UnityEngine;

namespace Utils {
    /// <summary>
    /// Utility class to add the <see cref="DontDestroyOnLoad"/> property to an object in the scene.
    /// </summary>
    public class DontDestroyOnLoad : MonoBehaviour {
        void Awake() {
            DontDestroyOnLoad(gameObject);
        }
    }
}
