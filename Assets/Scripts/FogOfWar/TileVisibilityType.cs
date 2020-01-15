namespace FogOfWar {
    public enum TileVisibilityType {
        // A fully clear tile.
        Visible,
        // A tile with medium fog. Previously visited.
        VisitedFog,
        // Tile with full fog. Not yet visited.
        NotVisitedFog
    }
}