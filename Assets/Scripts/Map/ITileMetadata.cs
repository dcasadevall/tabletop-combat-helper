namespace Map {
    /// <summary>
    /// Information additionally serialized to a tile in the grid, such as section connections, npc units to spawn,
    /// room wall vertices, etc..
    /// </summary>
    public interface ITileMetadata {
        /// <summary>
        /// If set, the section index this tile takes us to.
        /// </summary>
        uint? SectionConnection { get; }
    }
}