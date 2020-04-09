using System;
using Grid.Tiles;
using UnityEditor;
using UnityEngine;

namespace Utils.Editor.Widgets {
    public class SpritePoolArrayEditorGUI {
        private readonly int _arrayLength;
        private readonly string[] _labels;
        private readonly SpriteArray[] _spritePools;
        private readonly bool[] _foldoutsStates;
        private readonly string _helpBoxMessage;


        public SpritePoolArrayEditorGUI(int arrayLength,
                               string[] labels,
                               SpriteArray[] spritePools,
                               string helpBoxMessage = "") {
            _arrayLength = arrayLength;
            _labels = labels;
            _spritePools = spritePools;
            _foldoutsStates = new bool[arrayLength];
            _helpBoxMessage = helpBoxMessage;
        }


        public void DrawGUI() {
            if (_helpBoxMessage != "") {
                EditorGUILayout.HelpBox(_helpBoxMessage, MessageType.None);
            }
            
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 210;

            for (int i = 0; i < _arrayLength; i++) {
                EditorGUILayout.Space();
                string label = i < _labels.Length ? _labels[i] : "*Label*"; 
                _foldoutsStates[i] = EditorGUILayout.Foldout(_foldoutsStates[i], label);

                if (_foldoutsStates[i]) {
                    int count = EditorGUILayout.DelayedIntField("Number of Sprite Options",
                        _spritePools[i].spriteArray?.Length ?? 1);
                    if (count < 1) {
                        count = 1;
                    }

                    if (_spritePools[i].spriteArray == null || _spritePools[i].spriteArray.Length != count) {
                        Array.Resize(ref _spritePools[i].spriteArray, count);
                    }

                    for (int j = 0; j < count; j++) {
                        _spritePools[i].spriteArray[j] = (Sprite) EditorGUILayout.ObjectField("Option " + (j + 1),
                            _spritePools[i].spriteArray[j],
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