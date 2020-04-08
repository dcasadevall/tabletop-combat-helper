using System;
using UnityEditor;
using UnityEngine;

namespace Grid.Tiles {
    [CustomEditor(typeof(TerrainRandomTile))]
    public class TerrainTileEditor : Editor
    {
        private TerrainRandomTile Tile { get { return (target as TerrainRandomTile); } }
        private readonly bool[] _showFoldouts = new bool[15];
        private readonly String[] _labels = {
            "Filled",
            "Three Sides",
            "Two Sides and One Corner",
            "Two Adjacent Sides",
            "Two Opposite Sides",
            "One Side and Two Corners",
            "One Side and One Lower Corner",
            "One Side and One Upper Corner",
            "One Side",
            "Four Corners",
            "Three Corners",
            "Two Adjacent Corners",
            "Two Opposite Corners",
            "One Corner",
            "Empty"
        };

        public void OnEnable()
        {
            if (Tile.spritePools == null || Tile.spritePools.Length != 15)
            {
                Tile.spritePools = new SpriteArray[15];
                EditorUtility.SetDirty(Tile);
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Set which Sprite is shown based on the surroundings of the tile. " +
                                    "The labels make reference to which tiles on the sides " +
                                    "and corners around the tile don't have the same tile type. " +
                                    "\n\np.e. Filled means all tiles on sides and corners are different type. Empty means " +
                                    "no tiles on sides and corners are different type, so its surrounded by tiles of the " +
                                    "same type in all sides and corners (the naming of the labels makes no sense, and " +
                                    "uses an opposite convention of what would be intuitive or clear to reference which " +
                                    "tiles around are the same type for terrain blending purposes)." +
                                    "\n\nFor every surroundings permutation there's a pool of sprites " +
                                    "from which the sprite will be chosen randomly using the tile position as seed.", 
                MessageType.None);

            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 210;

            EditorGUI.BeginChangeCheck();
            
            for (int i = 0; i < 15; i++) {
                EditorGUILayout.Space();
                _showFoldouts[i] = EditorGUILayout.Foldout(_showFoldouts[i], _labels[i]);

                if (_showFoldouts[i]) {

                    int count = EditorGUILayout.DelayedIntField("Number of Sprite Options",
                        Tile.spritePools[i].spriteArray?.Length ?? 1);
                    if (count < 1) {
                        count = 1;
                    }

                    if (Tile.spritePools[i].spriteArray == null || Tile.spritePools[i].spriteArray.Length != count) {
                        Array.Resize(ref Tile.spritePools[i].spriteArray, count);
                    }

                    for (int j = 0; j < count; j++) {
                        Tile.spritePools[i].spriteArray[j] = (Sprite) EditorGUILayout.ObjectField("Option " + (j+1),
                            Tile.spritePools[i].spriteArray[j],
                            typeof(Sprite),
                            false,
                            null);
                    }
                }
            }

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(Tile);

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }
    }
}