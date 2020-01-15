using Grid;
using UnityEngine;

namespace Utils.Mesh {
    public class GridMeshGenerator : IGridMeshGenerator {
        /// <summary>
        /// Generates a new <see cref="Mesh"/> with the dimensions of the given <see cref="IGrid"/>.
        /// The generated vertices compose the tiles of the given grid.
        ///
        /// TODO: Currently, this assumes a grid size of 1. Should use <see cref="IGrid.TileSize"/>.
        /// </summary>
        public UnityEngine.Mesh Generate(IGrid grid) {
            var mesh = new UnityEngine.Mesh {
                name = "Procedural Grid"
            };

            var vertices = new Vector3[(grid.NumTilesX + 1) * (grid.NumTilesY + 1)];
            var uv = new Vector2[vertices.Length];
            var tangents = new Vector4[vertices.Length];
            var tangent = new Vector4(1f, 0f, 0f, -1f);
            for (int i = 0, y = 0; y <= grid.NumTilesY; y++) {
                for (int x = 0; x <= grid.NumTilesX; x++, i++) {
                    vertices[i] = new Vector3(x, y);
                    uv[i] = new Vector2((float) x / grid.NumTilesX, (float) y / grid.NumTilesY);
                    tangents[i] = tangent;
                }
            }

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.tangents = tangents;

            int[] triangles = new int[grid.NumTilesX * grid.NumTilesY * 6];
            for (int ti = 0, vi = 0, y = 0; y < grid.NumTilesY; y++, vi++) {
                for (int x = 0; x < grid.NumTilesX; x++, ti += 6, vi++) {
                    triangles[ti] = vi;
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                    triangles[ti + 4] = triangles[ti + 1] = vi + (int) grid.NumTilesX + 1;
                    triangles[ti + 5] = vi + (int) grid.NumTilesX + 2;
                }
            }

            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }
    }
}