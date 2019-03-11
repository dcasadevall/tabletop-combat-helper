using Math;
using UnityEngine;

namespace Grid.Positioning {
    public interface IRandomGridPositionProvider {
        /// <summary>
        /// Returns a set of unique random positions on the given grid.
        /// </summary>
        /// <param name="grid">The grid to iterate on.</param>
        /// <param name="maxDistanceFromCenter">If greater than 0, the returned tiles will be at most maxDistanceFromCenter tiles in x and y.</param>
        /// <param name="numTilesToGenerate">The number of tiles to be generated.</param>
        /// <returns></returns>
        IntVector2[] GetRandomUniquePositions(IGrid grid, int maxDistanceFromCenter, int numTilesToGenerate);
    }
}