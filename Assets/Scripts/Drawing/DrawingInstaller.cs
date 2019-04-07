using System;
using Drawing.UI;
using Grid;
using Grid.Serialized;
using UnityEngine;
using Zenject;

namespace Drawing {
    public class DrawingInstaller : MonoInstaller {
        [SerializeField]
        private DrawingViewController _drawingViewController;

        [SerializeField]
        private GameObject _drawableTilePrefab;

        [SerializeField]
        private DrawbleSpriteSettings _settings;

        public override void InstallBindings() {
            // Facade. Only expose IDrawingInputManagerInternal to IDrawingViewController.
            // Also, non lazily instantiate the VC so it immediately shows up.
            Container.Bind<IDrawingViewController>().FromSubContainerResolve()
                     .ByMethod(BindDrawingViewController)
                     .WithKernel()
                     .AsSingle()
                     .NonLazy();
            
            //Pooling
            Container.BindMemoryPool<DrawableTileBehaviour, DrawableTileBehaviour.Pool>().WithInitialSize(5)
                     .FromComponentInNewPrefab(_drawableTilePrefab).UnderTransformGroup("DrawableTiles");

            Container.Bind<IDrawableTileRegistry>().To<DrawableTileRegistry>().AsSingle();
            Container.Bind<DrawbleSpriteSettings>().FromInstance(_settings).AsSingle();
            Container.Bind<IFactory<int, Sprite>>().To<DrawableSpriteFactory>().AsSingle();
        }

        private void BindDrawingViewController(DiContainer container) {
            // container.Bind(typeof(ITickable), typeof(IDrawingInputManager)).To<DrawingInputManager>().AsSingle();
            container.Bind<DrawingInputManager>().AsSingle();
            container.Bind<IDrawingInputManager>().To<DrawingInputManager>().FromResolve();
            container.Bind<ITickable>().To<DrawingInputManager>().FromResolve();
            container.Bind<IDrawingViewController>().To<DrawingViewController>()
                     .FromComponentInNewPrefab(_drawingViewController).AsSingle();
        }
    }
}