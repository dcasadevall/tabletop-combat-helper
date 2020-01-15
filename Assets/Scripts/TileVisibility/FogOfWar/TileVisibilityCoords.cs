using Math;

namespace TileVisibility.FogOfWar {
    internal class TileVisibilityCoords {
        public readonly IntVector2 tileCoords;
        public readonly TileVisibilityType tileVisibilityType;
        
        public TileVisibilityCoords(IntVector2 tileCoords, TileVisibilityType tileVisibilityType) {
            this.tileCoords = tileCoords;
            this.tileVisibilityType = tileVisibilityType;
        }
    }
}