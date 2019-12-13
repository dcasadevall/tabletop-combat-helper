using System.Collections.Generic;
using CommandSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

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
                SceneContext sceneContext = rootGameObject.GetComponent<SceneContext>();
                if (sceneContext != null) {
                    CommandFactory.UnregisterSceneContainer(sceneContext.Container);
                }
                
                if (rootGameObject.activeSelf) {
                    rootGameObject.SetActive(false);
                    _activeGameObjects.Add(rootGameObject);
                }
            }
        }
        
        public void Reactivate() {
            foreach (var rootGameObject in _scene.GetRootGameObjects()) {
                SceneContext sceneContext = rootGameObject.GetComponent<SceneContext>();
                if (sceneContext != null) {
                    CommandFactory.RegisterSceneContainer(sceneContext.Container);
                }
                
                if (_activeGameObjects.Contains(rootGameObject)) {
                    rootGameObject.SetActive(true);
                }
            }
            
            _activeGameObjects.Clear();
        }
    }
}