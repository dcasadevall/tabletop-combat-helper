using CommandSystem;

namespace Drawing.Commands {
    public class ClearAllPixelsCommand : ICommand {
        public const string Id = "ClearAllPixels";
        
        private readonly IDrawableTileRegistry _drawableTileRegistry;
        private readonly ITexturePainter _texturePainter;

        public ClearAllPixelsCommand(IDrawableTileRegistry drawableTileRegistry, ITexturePainter texturePainter) {
            _drawableTileRegistry = drawableTileRegistry;
            _texturePainter = texturePainter;
        }
        
        public void Run() {
            foreach (var drawableTile in _drawableTileRegistry.GetAllTiles()) {
                _texturePainter.EraseAllPixels(drawableTile.Sprite);
            }
        }
    }
}