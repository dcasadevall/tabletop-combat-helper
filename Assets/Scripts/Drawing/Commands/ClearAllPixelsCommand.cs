using CommandSystem;

namespace Drawing.Commands {
    public class ClearAllPixelsCommand : ICommand<ClearAllPixelsCommandData> {
        private readonly IDrawableTileRegistry _drawableTileRegistry;
        private readonly ITexturePainter _texturePainter;

        public ClearAllPixelsCommand(IDrawableTileRegistry drawableTileRegistry, ITexturePainter texturePainter) {
            _drawableTileRegistry = drawableTileRegistry;
            _texturePainter = texturePainter;
        }
        
        public void Run(ClearAllPixelsCommandData commandData) {
            foreach (var drawableTile in _drawableTileRegistry.GetAllTiles()) {
                _texturePainter.EraseAllPixels(drawableTile.Sprite);
            }
        }
    }
}