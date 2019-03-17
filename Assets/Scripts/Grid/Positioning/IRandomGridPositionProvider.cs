using Math;
using UnityEngine;

namespace Grid.Positioning {
    public interface IRandomGridPositionProvider {
        /// <summary>
        /// Returns a set of unique random positions on the grid in current context.
        /// </summary>
        /// <param name="maxDistanceFromCenter">If greater than 0, the returned tiles will be at most maxDistanceFromCenter tiles in x and y.</param>
        /// <param name="numTilesToGenerate">The number of tiles to be generated.</param>
        /// <returns></returns>
        IntVector2[] GetRandomUniquePositions(int maxDistanceFromCenter, int numTilesToGenerate);
    }
}