using CommandSystem;

namespace Drawing.Commands {
    public class PaintPixelCommand : ICommand<PaintPixelData> {
        public const string Id = "ClearAllPixels";
        
        private readonly IDrawableTileRegistry _drawableTileRegistry;
        private readonly ITexturePainter _texturePainter;

        public PaintPixelCommand(IDrawableTileRegistry drawableTileRegistry, ITexturePainter texturePainter) {
            _drawableTileRegistry = drawableTileRegistry;
            _texturePainter = texturePainter;
        }
        
        public void Run(PaintPixelData data) {
            IDrawableTile drawableTile = _drawableTileRegistry.GetDrawableTileAtCoordinates(data._drawableTileCoords);
            _texturePainter.PaintPixel(drawableTile.Sprite, data._pixelPosition, data._paintParams);
        }
    }
}