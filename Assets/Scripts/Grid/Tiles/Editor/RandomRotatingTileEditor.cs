using System;
using System.Collections;
using System.Collections.Generic;
using Grid.Tiles;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RandomRotatingTile))]
    public class RandomRotatingTileEditor : Editor {
        private SerializedProperty m_Color;
        private SerializedProperty m_ColliderType;

        private RandomRotatingTile Tile {
            get {
                return (target as RandomRotatingTile);
            }
        }

        public void OnEnable() {
            m_Color = serializedObject.FindProperty("m_Color");
            m_ColliderType = serializedObject.FindProperty("m_ColliderType");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            Tile.weightedRandomEnabled = EditorGUILayout.Toggle("Weighted Random", Tile.weightedRandomEnabled);

            Tile.randomRotationEnabled = EditorGUILayout.Toggle("Random Rotation", Tile.randomRotationEnabled);
            if (Tile.randomRotationEnabled) {
                Tile.rotationIntervals = EditorGUILayout.FloatField("Rotation Intervals", Tile.rotationIntervals);
            }
            Tile.randomFlippingEnabled = EditorGUILayout.Toggle("Random Flip", Tile.randomFlippingEnabled);
            
            int count = EditorGUILayout.DelayedIntField("Number of Sprites",
                Tile.weightedSprites != null ? Tile.weightedSprites.Length : 0);
            if (count < 0)
                count = 0;
            if (Tile.weightedSprites == null || Tile.weightedSprites.Length != count) {
                Array.Resize(ref Tile.weightedSprites, count);
            }

            if (count == 0)
                return;

            EditorGUILayout.LabelField("Place random sprites.");
            EditorGUILayout.Space();

            for (int i = 0; i < count; i++) {
                Tile.weightedSprites[i].Sprite = (Sprite) EditorGUILayout.ObjectField("Sprite " + (i + 1),
                    Tile.weightedSprites[i].Sprite,
                    typeof(Sprite),
                    false,
                    null);
                if (Tile.weightedRandomEnabled) {
                    Tile.weightedSprites[i].Weight =
                        EditorGUILayout.IntField("Weight" + (i + 1), Tile.weightedSprites[i].Weight);
                }
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_Color);
            EditorGUILayout.PropertyField(m_ColliderType);

            if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(Tile);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
