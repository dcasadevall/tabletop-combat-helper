using System;
using CommandSystem;
using CommandSystem.Installers;
using Drawing.Commands;
using Drawing.DrawableTiles;
using Drawing.Input;
using Drawing.TexturePainter;
using Drawing.UI;
using Grid;
using Grid.Commands;
using Grid.Serialized;
using UnityEngine;
using Zenject;

namespace Drawing {
    /// <summary>
    /// This installer needs to be in a context which is descendant of <see cref="DrawingTexturesInstaller"/>.
    /// </summary>
    public class DrawingSectionInstaller : MonoInstaller {
        [SerializeField]
        private GameObject _drawableTilePrefab;
        
        public override void InstallBindings() {
            Container.BindInterfacesTo<DrawingInputManager>().AsSingle();
            Container.Bind<IDrawableTileRegistry>().To<DrawableTileRegistry>().AsSingle();
                        
            Container.BindMemoryPool<DrawableTileBehaviour, DrawableTileBehaviour.Pool>()
                     .WithInitialSize(5)
                     .FromComponentInNewPrefab(_drawableTilePrefab)
                     .UnderTransformGroup("DrawableTiles");
            
            // Commands
            CommandsInstaller.Install<DrawingCommandsInstaller>(Container);
        }
    }
}