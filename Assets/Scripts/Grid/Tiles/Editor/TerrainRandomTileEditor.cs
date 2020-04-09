using System;
using UnityEditor;
using UnityEngine;
using Utils.Editor.Widgets;

namespace Grid.Tiles {
    [CustomEditor(typeof(TerrainRandomTile))]
    public class TerrainTileEditor : Editor
    {
        private const int kSpritePoolsLength = 15;
        private TerrainRandomTile Tile {
            get {
                return (target as TerrainRandomTile);
            }
        }
        
        private SpritePoolArrayEditorGUI _spritePoolArrayEditorGui;
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
        
        private const string kHelpBoxMessage = "Set which Sprite is shown based on the surroundings of the tile. " +
                                               "The labels make reference to which tiles on the sides " +
                                               "and corners around the tile don't have the same tile type. " +
                                               "\n\np.e. Filled means all tiles on sides and corners are different type. Empty means " +
                                               "no tiles on sides and corners are different type, so its surrounded by tiles of the " +
                                               "same type in all sides and corners (the naming of the labels makes no sense, and " +
                                               "uses an opposite convention of what would be intuitive or clear to reference which " +
                                               "tiles around are the same type for terrain blending purposes)." +
                                               "\n\nFor every surroundings permutation there's a pool of sprites " +
                                               "from which the sprite will be chosen randomly using the tile position as seed.";
        
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