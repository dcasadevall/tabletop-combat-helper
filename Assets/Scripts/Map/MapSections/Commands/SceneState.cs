using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Map.MapSections.Commands {
    /// <summary>
    /// Defines the current state of a scene. Mainly its objects and whether they should be active / inactive.
    /// </summary>
    internal sealed class SceneState {
        private readonly Scene _scene;
        private readonly HashSet<GameObject> _activeGameObjects = new HashSet<GameObject>();

        public SceneState(Scene scene) {
            _scene = scene;
        }

        public void Deactivate() {
            foreach (var rootGameObject in _scene.GetRootGameObjects()) {
                if (rootGameObject.activeSelf) {
                    rootGameObject.SetActive(false);
                    _activeGameObjects.Add(rootGameObject);
                }
            }
        }
        
        public void Reactivate() {
            foreach (var rootGameObject in _scene.GetRootGameObjects()) {
                if (_activeGameObjects.Contains(rootGameObject)) {
                    rootGameObject.SetActive(true);
                }
            }
            
            _activeGameObjects.Clear();
        }
    }
}