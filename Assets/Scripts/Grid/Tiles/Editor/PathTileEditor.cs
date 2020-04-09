﻿using System;
using UnityEditor;
using Utils.Editor;
using Utils.Editor.Widgets;


namespace Grid.Tiles {
    [CustomEditor(typeof(PathTile))]
    public class PathTileEditor : Editor {
        private const int kSpritePoolsLength = 6;
        private PathTile Tile {
            get {
                return (target as PathTile);
            }
        }

        private SpritePoolArrayEditorGUI _spritePoolArrayEditorGUI;
        private readonly String[] _labels = {
            "Single",
            "Start",
            "End",
            "Straight",
            "90 turn right",
            "90 turn left"
        };
        
        private const string kHelpBoxMessage = "Set which Sprite is shown based on the position of the" +
                                "previous tile of the path. The names are for a path that starts on the" +
                                "top and goes down. The elements of the inverse path (the same" +
                                "path flipped horizontally) are arranged automatically using the same sprites.";


        public void OnEnable() {
            if (Tile.spritePools == null || Tile.spritePools.Length != kSpritePoolsLength) {
                Tile.spritePools = new SpriteArray[kSpritePoolsLength];
                EditorUtility.SetDirty(Tile);
            }
            _spritePoolArrayEditorGUI = new SpritePoolArrayEditorGUI(kSpritePoolsLength, _labels, Tile.spritePools, kHelpBoxMessage);
        }

        public override void OnInspectorGUI() {
            EditorGUI.BeginChangeCheck();
            _spritePoolArrayEditorGUI.DrawGUI();
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(Tile);
        }
    }
   
}