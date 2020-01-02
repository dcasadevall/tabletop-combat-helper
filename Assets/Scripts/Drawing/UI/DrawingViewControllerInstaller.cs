using UI;
using UnityEngine;
using Zenject;

namespace Drawing.UI {
    /// <summary>
    /// Installer for the drawing UI, which can have a different context than the drawing input / tile registry, etc..
    /// i.e: Our viewcontroller can appear in a parent scene, which loads subscenes to draw on.
    /// </summary>
    public class DrawingViewControllerInstaller : MonoInstaller {
        public const string DRAWING_OVERLAY_ID = "Drawing";
        
        [SerializeField]
        private GameObject _drawingViewController;

        public override void InstallBindings() {
            // TODO: Avoid exposing this implementation globally
            // We need to do this here so we can use "FromResolve" in order to inject different interfaces
            // to the same implementation (one with id) later on.
            Container.Bind<DrawingViewController>()
                     .FromComponentInNewPrefab(_drawingViewController)
                     .AsSingle()
                     .NonLazy();

            Container.Bind<IDrawingViewController>().To<DrawingViewController>().FromResolve();
            Container.Bind<IDismissNotifyingViewController>().WithId(DRAWING_OVERLAY_ID).To<DrawingViewController>().FromResolve();
        }
    }
}