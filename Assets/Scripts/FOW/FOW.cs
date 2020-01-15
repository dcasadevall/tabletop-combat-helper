using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOW : MonoBehaviour {
    public int mapSizeX, mapSizeY;
    public Vector3[] FOWMapArray;

    private Color32 fogVertexColor = new Color32(0, 0, 0, 0);
    private Color32 outOfViewVertexColor = new Color32(0, 0, 0, 130);
    private Color32 inViewVertexColor = new Color32(0, 0, 0, 255);

    private Color32 selectedColor;
    private int tileVertexIndex = 0;

    void Start() {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Color32[] vertexColors = new Color32[vertices.Length];

        Debug.Log(vertices.Length);

        for (int i = 0; i < FOWMapArray.Length; i += 1) {
            Debug.Log("i: " + i);
            Debug.Log("FOW array: " + FOWMapArray[i]);

            if (FOWMapArray[i].z == 0) {
                selectedColor = fogVertexColor;
            } else if (FOWMapArray[i].z == 1) {
                selectedColor = outOfViewVertexColor;
            } else {
                selectedColor = inViewVertexColor;
            }

            // add 1 to map size to account for N+1 vertices in the tiles
            tileVertexIndex = (int) FOWMapArray[i].y * (mapSizeX + 1) + (int) FOWMapArray[i].x;

            // get 4 verts
            vertexColors[tileVertexIndex] = selectedColor;
            vertexColors[tileVertexIndex + 1] = selectedColor;
            vertexColors[tileVertexIndex + mapSizeX + 1] = selectedColor;
            vertexColors[tileVertexIndex + mapSizeX + 2] = selectedColor;
        }

        mesh.colors32 = vertexColors;
    }
}