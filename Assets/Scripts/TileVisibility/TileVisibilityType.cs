namespace TileVisibility {
    public enum TileVisibilityType {
        // A tile visible by any of the player units.
        Visible,
        // A tile not currently in sight of any player units, which had been previously visible.
        VisitedNotInSight,
        // A tile which has never been visible.
        NotVisited
    }
}