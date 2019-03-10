
using UnityEngine;

namespace Grid.Serialized {
    public interface IGridData {
        /// <summary>
        /// Number of tiles in the X axis.
        /// </summary>
        uint NumTilesX { get; }
        /// <summary>
        /// Number of tiles in the Y axis.
        /// </summary>
        uint NumTilesY { get; }
        /// <summary>
        /// If specified, the grid's origin will be set to this world position.
        /// If not specified, the grid's origin will be (-NumTilesX / 2.0f, -NumTilesY / 2.0f).
        /// e.g: The grid will have it's Center at 0, 0 by default.
        /// </summary>
        Vector2? OriginWorldPosition { get; }
    }
}