using System;
using Map.Serialized;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace {
    public class Goldmane_Loader {
        [MenuItem("Pathfinder/Populate Goldmane")]
        public static void PopulateItem() {
            MapData mapData = (MapData)Selection.activeObject;
            Debug.Log(mapData);

            int i = 0;
            for (int row = 1; row <= 8; row++) {
                for (int column = 1; column <= 13; column++) {
                    string spritePath = $"Sprites/Goldmane Manor 2nd Floor/Goldmane Manor 2nd Floor {row}-{column}";
                    Sprite sprite = Resources.Load<Sprite>(spritePath);

                    mapData.sections[1].sprites[i] = sprite;
                    i++;
                }
            }
        }
    }
}