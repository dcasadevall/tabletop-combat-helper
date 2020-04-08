using System;
using UnityEditor;
using UnityEngine;

namespace Grid.Tiles {
    [CustomEditor(typeof(PathTile))]
    public class PathTileEditor : Editor {
        private const int kSpritePoolsLength = 6;
        private PathTile Tile {
            get {
                return (target as PathTile);
            }
        }

        private readonly bool[] _showFoldouts = new bool[kSpritePoolsLength];

        private readonly String[] _labels = {
            "Single",
            "Start",
            "End",
            "Straight",
            "90 turn right",
            "90 turn left"
        };

        public void OnEnable() {
            if (Tile.spritePools == null || Tile.spritePools.Length != kSpritePoolsLength) {
                Tile.spritePools = new SpriteArray[kSpritePoolsLength];
                EditorUtility.SetDirty(Tile);
            }
        }

        public override void OnInspectorGUI() {
            EditorGUILayout.HelpBox("Set which Sprite is shown based on the position of the" +
                                    "previous tile of the path. The names are for a path that starts on the" +
                                    "top and goes down. The elements of the inverse path (the same" +
                                    "path flipped horizontally) are arranged automatically using the same sprites.",
                MessageType.None);

            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 210;

            EditorGUI.BeginChangeCheck();

            for (int i = 0; i < kSpritePoolsLength; i++) {
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
                        Tile.spritePools[i].spriteArray[j] = (Sprite) EditorGUILayout.ObjectField("Option " + (j + 1),
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