using System;
using CommandSystem;
using Drawing.Commands;
using Drawing.DrawableTiles;
using Drawing.Input;
using Drawing.TexturePainter;
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
                     .AsSingle()
                     .NonLazy();

            Container.Bind<ITexturePainter>().To<TexturePainter.TexturePainter>().FromSubContainerResolve()
                     .ByMethod(BindTexturePainter).AsSingle();
            
            //Pooling
            Container.BindMemoryPool<DrawableTileBehaviour, DrawableTileBehaviour.Pool>().WithInitialSize(5)
                     .FromComponentInNewPrefab(_drawableTilePrefab).UnderTransformGroup("DrawableTiles");

            Container.Bind<IDrawableTileRegistry>().To<DrawableTileRegistry>().AsSingle();
            Container.Bind<DrawbleSpriteSettings>().FromInstance(_settings).AsSingle();
            Container.Bind<IFactory<int, Sprite>>().To<DrawableSpriteFactory>().AsSingle();
            
            // Commands
            Container.Install<DrawingCommandsInstaller>();
        }

        private void BindDrawingViewController(DiContainer container) {
            container.Bind<IDrawingInputManager>().To<DrawingInputManager>().AsSingle();
            container.Bind<IDrawingViewController>().To<DrawingViewController>()
                     .FromComponentInNewPrefab(_drawingViewController).AsSingle();
        }
        
        private void BindTexturePainter(DiContainer container) {
            container.Bind<TexturePainter.TexturePainter>().AsSingle();
            container.BindFactory<Sprite, ISpriteState, SpriteState.Factory>().To<SpriteState>()
                     .AsSingle();
        }
    }
}