using System.Collections.Generic;
using CommandSystem;
using Drawing.DrawableTiles;
using Drawing.TexturePainter;

namespace Drawing.Commands {
    public class ClearAllPixelsCommand : ICommand<ClearAllPixelsCommandData> {
        private readonly IDrawableTileRegistry _drawableTileRegistry;
        private readonly ITexturePainter _texturePainter;
        
        // This command is bound as Transient Scope, so we can hold state to restore on Undo()
        private Dictionary<IDrawableTile, ISpriteState> _spriteStates = new Dictionary<IDrawableTile, ISpriteState>();
        
        public bool IsInitialGameStateCommand {
            get {
                return false;
            }
        }

        public ClearAllPixelsCommand(IDrawableTileRegistry drawableTileRegistry, ITexturePainter texturePainter) {
            _drawableTileRegistry = drawableTileRegistry;
            _texturePainter = texturePainter;
        }

        public void Run(ClearAllPixelsCommandData commandData) {
            foreach (var drawableTile in _drawableTileRegistry.GetAllTiles()) {
                // Save state of this tile and add it to the command state for Undo()
                _spriteStates[drawableTile] = _texturePainter.SaveState(drawableTile.Sprite);
                _texturePainter.EraseAllPixels(drawableTile.Sprite);
            }
        }

        public void Undo(ClearAllPixelsCommandData data) {
            // This assumes that the drawable tile lifecycle is perennial after it has been drawn once.
            foreach (var spriteState in _spriteStates) {
                spriteState.Value.RestoreState(spriteState.Key.Sprite);
            }
        }
    }
}