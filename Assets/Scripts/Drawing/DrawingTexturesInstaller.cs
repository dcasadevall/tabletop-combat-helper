using Drawing.DrawableTiles;
using Drawing.TexturePainter;
using UnityEngine;
using Zenject;

namespace Drawing {
    /// <summary>
    /// This installer is separate from <see cref="DrawingSectionInstaller"/> because DrawableTiles
    /// should be reused across different Drawing contexts.
    /// 
    /// I.e: Our map may have different sections. Each section has its own drawing context, but they all
    /// share the same drawable tile pool.
    /// </summary>
    public class DrawingTexturesInstaller : MonoInstaller {
        [SerializeField]
        private DrawbleSpriteSettings _settings;

        public override void InstallBindings() {
            // Texture Painter and sprites facade
            Container.Bind<ITexturePainter>().To<TexturePainter.TexturePainter>().FromSubContainerResolve()
                     .ByMethod(BindTexturePainter).AsSingle();
            Container.Bind<DrawbleSpriteSettings>().FromInstance(_settings).AsSingle();
            Container.Bind<IFactory<Sprite>>().To<DrawableSpriteFactory>().AsSingle();

        }
        
        private void BindTexturePainter(DiContainer container) {
            container.Bind<TexturePainter.TexturePainter>().AsSingle();
            container.BindFactory<Sprite, ISpriteState, SpriteState.Factory>().To<SpriteState>()
                     .AsSingle();
        }
    }
}