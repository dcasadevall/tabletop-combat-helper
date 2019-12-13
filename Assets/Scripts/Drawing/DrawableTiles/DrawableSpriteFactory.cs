using Logging;
using UnityEngine;
using Zenject;
using ILogger = Logging.ILogger;

namespace Drawing.DrawableTiles {
    public class DrawableSpriteFactory : IFactory<Sprite> {
        private readonly DrawbleSpriteSettings _drawbleSpriteSettings;
        private readonly ILogger _logger;
        private int _numTiles = 0;
            
        public DrawableSpriteFactory(DrawbleSpriteSettings drawbleSpriteSettings, ILogger logger) {
            _drawbleSpriteSettings = drawbleSpriteSettings;
            _logger = logger;
        } 
            
        public Sprite Create() {
            string spritePath = string.Format(_drawbleSpriteSettings.format,
                                              _drawbleSpriteSettings.path,
                                              _numTiles);
            _numTiles++;
            
            Sprite sprite = Resources.Load<Sprite>(spritePath);
            if (sprite == null) {
                _logger.LogError(LoggedFeature.Drawing, "Sprite not found: {0}.", spritePath);
            }

            return sprite;
        }
    }
}