using Math;
using UnityEngine;

namespace Grid.Positioning {
    public interface IRandomGridPositionProvider {
        /// <summary>
        /// Returns a set of unique random positions on the grid in current context, starting at the given tile.
        /// </summary>
        /// <param name="startTile">The tile to start generating coords from.</param>
        /// <param name="maxDistance">If greater than 0, the returned tiles will be at most maxDistance tiles in x and y.</param>
        /// <param name="numTilesToGenerate">The number of tiles to be generated.</param>
        /// <returns></returns>
        IntVector2[] GetRandomUniquePositions(IntVector2 startTile, int maxDistance, int numTilesToGenerate);
    }
}