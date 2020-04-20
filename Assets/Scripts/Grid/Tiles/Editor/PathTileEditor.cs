using System;
using UnityEditor;
using Utils.Editor.Widgets;

namespace Grid.Tiles {
    [CustomEditor(typeof(PathTile.PathTile))]
    public class PathTileEditor : UnityEditor.Editor {
        private const int kSpritePoolsLength = 15;
        private PathTile.PathTile Tile {
            get {
                return (target as PathTile.PathTile);
            }
        }

        private SpritePoolArrayEditorGUI _spritePoolArrayEditorGui;
        private readonly String[] _labels = {
            "Single",
            "Start",
            "End",
            "Straight",
            "turn right",
            "turn left",
            "Diagonal start",
            "Diagonal end",
            "Diagonal",
            "Half turn right",
            "Half turn left",
            "Right diagonal down",
            "Left diagonal down",
            "Corner right side",
            "Corner left side"
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
            _spritePoolArrayEditorGui = new SpritePoolArrayEditorGUI(kSpritePoolsLength, _labels, Tile.spritePools, kHelpBoxMessage);
        }

        public override void OnInspectorGUI() {
            EditorGUI.BeginChangeCheck();
            _spritePoolArrayEditorGui.DrawGUI();
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(Tile);
        }
    }
   
}