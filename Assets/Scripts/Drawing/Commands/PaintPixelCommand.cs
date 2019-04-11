using CommandSystem;
using Drawing.DrawableTiles;
using Drawing.TexturePainter;

namespace Drawing.Commands {
    public class PaintPixelCommand : ICommand<PaintPixelData> {
        private readonly IDrawableTileRegistry _drawableTileRegistry;
        private readonly ITexturePainter _texturePainter;
        
        // This command is bound as Transient Scope, so we can hold state to restore on Undo()
        private ISpriteState _spriteState;

        public PaintPixelCommand(IDrawableTileRegistry drawableTileRegistry, ITexturePainter texturePainter) {
            _drawableTileRegistry = drawableTileRegistry;
            _texturePainter = texturePainter;
        }
        
        public void Run(PaintPixelData data) {
            IDrawableTile drawableTile = _drawableTileRegistry.GetDrawableTileAtCoordinates(data._drawableTileCoords);
            _spriteState = _texturePainter.SaveState(drawableTile.Sprite);
            _texturePainter.PaintPixel(drawableTile.Sprite, data._pixelPosition, data._paintParams);
        }

        public void Undo(PaintPixelData data) {
            IDrawableTile drawableTile = _drawableTileRegistry.GetDrawableTileAtCoordinates(data._drawableTileCoords);
            _spriteState.RestoreState(drawableTile.Sprite);
        }
    }
}