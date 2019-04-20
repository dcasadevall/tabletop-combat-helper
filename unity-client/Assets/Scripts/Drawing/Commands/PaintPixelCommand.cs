using System;
using CommandSystem;
using Drawing.DrawableTiles;
using Drawing.TexturePainter;
using UniRx;

namespace Drawing.Commands {
    public class PaintPixelCommand : ICommand {
        private readonly PaintPixelData _data;
        private readonly IDrawableTileRegistry _drawableTileRegistry;
        private readonly ITexturePainter _texturePainter;
        
        // This command is bound as Transient Scope, so we can hold state to restore on Undo()
        private ISpriteState _spriteState;
                
        public bool IsInitialGameStateCommand {
            get {
                return false;
            }
        }

        public PaintPixelCommand(PaintPixelData data, IDrawableTileRegistry drawableTileRegistry,
                                 ITexturePainter texturePainter) {
            _data = data;
            _drawableTileRegistry = drawableTileRegistry;
            _texturePainter = texturePainter;
        }
        
        public IObservable<Unit> Run() {
            IDrawableTile drawableTile = _drawableTileRegistry.GetDrawableTileAtCoordinates(_data._drawableTileCoords);
            _spriteState = _texturePainter.SaveState(drawableTile.Sprite);
            _texturePainter.PaintPixel(drawableTile.Sprite, _data._pixelPosition, _data._paintParams);

            return Observable.ReturnUnit();
        }

        public void Undo() {
            IDrawableTile drawableTile = _drawableTileRegistry.GetDrawableTileAtCoordinates(_data._drawableTileCoords);
            _spriteState.RestoreState(drawableTile.Sprite);
        }
    }
}