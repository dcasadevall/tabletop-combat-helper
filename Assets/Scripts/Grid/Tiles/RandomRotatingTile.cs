﻿using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
#if UNITY_EDITOR

#endif

namespace Grid.Tiles {
    /// <summary>
    /// Random Tiles are tiles which pseudo-randomly pick a sprite from a given list of sprites and a target location, and displays that sprite.
    /// The Sprite displayed for the Tile is randomized based on its location and will be fixed for that particular location.
    /// The random pick can be weighted.
    /// Also, if enabled, a random rotation and flipping is added to the sprite.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "New Random Rotating Tile", menuName = "Tiles/Random Rotating Tile")]
    public class RandomRotatingTile : Tile {

        /// <summary>
        /// Determines if the weighted random should be applied;
        /// </summary>
        [SerializeField] public bool weightedRandom = false;
        
        /// <summary>
        /// If random rotation is applied to the sprite
        /// </summary>
        [SerializeField] public bool randomRotationEnabled = true;

        /// <summary>
        /// If random flipping in x and y axis is applied to the sprite
        /// </summary>
        [SerializeField] public bool randomFlippingEnabled = true;
        
        /// <summary>
        /// IF random rotation is true, if there are fixed angle intervals, like every 90 degrees. 
        /// </summary>
        [SerializeField] public float rotationIntervals = 0f;

        

        /// <summary>
        /// The Sprites used for randomizing output.
        /// </summary>
        [SerializeField] public Sprite[] m_Sprites;

        /// <summary>
        /// Retrieves any tile rendering data from the scripted tile.
        /// </summary>
        /// <param name="location">Position of the Tile on the Tilemap.</param>
        /// <param name="tileMap">The Tilemap the tile is present on.</param>
        /// <param name="tileData">Data to render the tile.</param>
        public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData) {
            base.GetTileData(location, tileMap, ref tileData);

            tileData.transform = Matrix4x4.identity;
            tileData.color = Color.white;
            
            if ((m_Sprites != null) && (m_Sprites.Length > 0)) {
                long hash = location.x;
                hash = (hash + 0xabcd1234) + (hash << 15);
                hash = (hash + 0x0987efab) ^ (hash >> 11);
                hash ^= location.y;
                hash = (hash + 0x46ac12fd) + (hash << 7);
                hash = (hash + 0xbe9730af) ^ (hash << 11);
                var oldState = Random.state;
                Random.InitState((int) hash);
                tileData.sprite = m_Sprites[(int) (m_Sprites.Length * Random.value)];

                
                
                Quaternion rotation = Quaternion.identity;
                if (randomRotationEnabled) {
                    rotation = GetRandomRotation(rotationIntervals, Random.value);
                }
                
                Vector3 scale = Vector3.one;
                if (randomFlippingEnabled) {
                    scale = GetScaleRandomFlipping(Random.value);
                }

                Random.state = oldState;
                
                tileData.transform = Matrix4x4.TRS(Vector3.zero, rotation, scale);
                tileData.flags = TileFlags.LockTransform | TileFlags.LockColor;
                tileData.colliderType = Tile.ColliderType.Sprite;
            }
        }

        /// <summary>
        /// Creates an angle from a random 0 to 1 value with possible fixed angle intervals.
        /// </summary>
        /// <param name="fixedIntervals">The degrees that the angle should snap to, 0 means no snapping</param>
        /// <param name="randomValue">random value from which to extract the angle</param>
        /// <returns></returns>
        private Quaternion GetRandomRotation(float fixedIntervals, float randomValue) {
            float randomAngle = 360 * randomValue;
            if (fixedIntervals > 0) {
                int clampedVal = (int)(randomAngle / fixedIntervals);
                randomAngle = clampedVal * fixedIntervals;
            }
            return Quaternion.Euler(0f, 0f, randomAngle);
        }

        private Vector3 GetScaleRandomFlipping(float randomValue) {
            // Provides a bit for every axis and gives 1 or 0 to each one randomly.
            int axisToFlip = (int) (3 * randomValue);
            Vector3 scaleFlipping = Vector3.one;
            switch (axisToFlip) {
                case 0:
                    break;
                case 1:
                    scaleFlipping.x = -1;
                    break;
                case 2:
                    scaleFlipping.y = -1;
                    break;
            }
            return scaleFlipping;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(RandomRotatingTile))]
    public class RandomTileEditor : Editor {
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

            Tile.randomRotationEnabled = EditorGUILayout.Toggle("Random Rotation", Tile.randomRotationEnabled);
            if (Tile.randomRotationEnabled) {
                Tile.rotationIntervals = EditorGUILayout.FloatField("Rotation Intervals", Tile.rotationIntervals);
            }
            Tile.randomFlippingEnabled = EditorGUILayout.Toggle("Random Flip", Tile.randomFlippingEnabled);
            
            int count = EditorGUILayout.DelayedIntField("Number of Sprites",
                Tile.m_Sprites != null ? Tile.m_Sprites.Length : 0);
            if (count < 0)
                count = 0;
            if (Tile.m_Sprites == null || Tile.m_Sprites.Length != count) {
                Array.Resize<Sprite>(ref Tile.m_Sprites, count);
            }

            if (count == 0)
                return;

            EditorGUILayout.LabelField("Place random sprites.");
            EditorGUILayout.Space();

            for (int i = 0; i < count; i++) {
                Tile.m_Sprites[i] = (Sprite) EditorGUILayout.ObjectField("Sprite " + (i + 1),
                    Tile.m_Sprites[i],
                    typeof(Sprite),
                    false,
                    null);
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
#endif
}