using UnityEngine;
using Zenject;

namespace Drawing.UI {
    /// <summary>
    /// Installer for the drawing UI, which can have a different context than the drawing input / tile registry, etc..
    /// i.e: Our viewcontroller can appear in a parent scene, which loads subscenes to draw on.
    /// </summary>
    public class DrawingViewControllerInstaller : MonoInstaller {
        [SerializeField]
        private GameObject _drawingViewController;

        public override void InstallBindings() {
            Container.Bind<IDrawingViewController>().To<DrawingViewController>()
                     .FromComponentInNewPrefab(_drawingViewController).AsSingle().NonLazy();
        }
    }
}