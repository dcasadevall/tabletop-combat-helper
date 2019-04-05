using Logging;
using UnityEngine;
using Zenject;
using ILogger = Logging.ILogger;

namespace Drawing {
    public class DrawableSpriteFactory : IFactory<int, Sprite> {
        private readonly DrawbleSpriteSettings _drawbleSpriteSettings;
        private readonly ILogger _logger;
            
        public DrawableSpriteFactory(DrawbleSpriteSettings drawbleSpriteSettings, ILogger logger) {
            _drawbleSpriteSettings = drawbleSpriteSettings;
            _logger = logger;
        } 
            
        public Sprite Create(int numTile) {
            string spritePath = string.Format(_drawbleSpriteSettings.format,
                                              _drawbleSpriteSettings.path,
                                              numTile);
            Sprite sprite = Resources.Load<Sprite>(spritePath);
            if (sprite == null) {
                _logger.LogError(LoggedFeature.Drawing, "Sprite not found: {0}.", spritePath);
            }

            return sprite;
        }
    }
}