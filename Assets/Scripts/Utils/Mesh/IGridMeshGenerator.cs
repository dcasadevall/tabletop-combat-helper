using Grid;

namespace Utils.Mesh {
    public interface IGridMeshGenerator {
        /// <summary>
        /// Generates a mesh covering the given grid. The generated mesh should be anchored at the grid origin (0, 0).
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        UnityEngine.Mesh Generate(IGrid grid);
    }
}