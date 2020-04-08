using Grid.Tiles;
using UnityEditor;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Utils.Editor {
    public static class TileEditorWidgets {
        static void SpritePoolArray(int arrayLength,
                                      string helpBoxMessage,
                                      string[] labels,
                                      SpriteArray[] spritePools) {
            
            EditorGUILayout.HelpBox(helpBoxMessage, MessageType.None);

            bool[] showFoldouts = new bool[arrayLength];
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 210;

            for (int i = 0; i < arrayLength; i++) {
                EditorGUILayout.Space();
                showFoldouts[i] = EditorGUILayout.Foldout(showFoldouts[i], labels[i]);

                if (showFoldouts[i]) {
                    int count = EditorGUILayout.DelayedIntField("Number of Sprite Options",
                        spritePools[i].spriteArray?.Length ?? 1);
                    if (count < 1) {
                        count = 1;
                    }

                    if (spritePools[i].spriteArray == null || spritePools[i].spriteArray.Length != count) {
                        Array.Resize(ref spritePools[i].spriteArray, count);
                    }

                    for (int j = 0; j < count; j++) {
                        spritePools[i].spriteArray[j] = (Sprite) EditorGUILayout.ObjectField("Option " + (j + 1),
                            spritePools[i].spriteArray[j],
                            typeof(Sprite),
                            false,
                            null);
                    }
                }
            }
            EditorGUIUtility.labelWidth = oldLabelWidth;
        }
    }
}