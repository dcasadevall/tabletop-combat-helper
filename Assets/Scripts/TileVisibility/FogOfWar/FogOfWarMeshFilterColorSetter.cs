using System.Collections.Generic;
using Grid;
using Math;
using UnityEngine;

namespace TileVisibility.FogOfWar {
    internal class FogOfWarMeshFilterColorSetter {
        private static Color32 fogVertexColor = new Color32(0, 0, 0, 0);
        private static Color32 outOfViewVertexColor = new Color32(0, 0, 0, 130);
        private static Color32 inViewVertexColor = new Color32(0, 0, 0, 255);
        
        /// <summary>
        /// Given a dictionary of tile coords to tile visibility, sets the colors in the given mesh.
        /// Used for gradual updates.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="visibilityMap"></param>
        /// <param name="grid"></param>
        public void SetColors(Mesh mesh, Dictionary<IntVector2, TileVisibilityType> visibilityMap, IGrid grid) {
            Vector3[] vertices = mesh.vertices;
            Color32[] vertexColors = new Color32[vertices.Length];
            
            foreach (var kvp in visibilityMap) {
                Color32 selectedColor;
                if (kvp.Value == TileVisibilityType.NotVisited) {
                    selectedColor = fogVertexColor;
                } else if (kvp.Value == TileVisibilityType.VisitedNotInSight) {
                    selectedColor = outOfViewVertexColor;
                } else {
                    selectedColor = inViewVertexColor;
                }

                // add 1 to map size to account for N+1 vertices in the tiles
                int tileVertexIndex = kvp.Key.y * ((int) grid.NumTilesX + 1) +
                                      kvp.Key.x;

                // get 4 verts
                vertexColors[tileVertexIndex] = selectedColor;
                vertexColors[tileVertexIndex + 1] = selectedColor;
                vertexColors[tileVertexIndex + grid.NumTilesX + 1] = selectedColor;
                vertexColors[tileVertexIndex + grid.NumTilesX + 2] = selectedColor;    
            }

            mesh.colors32 = vertexColors;
        }
    }
}