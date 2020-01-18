using System;
using UnityEngine;

namespace Map.Serialized {
    [Serializable]
    public class TileMetadataPair {
        public Vector2 tileCoords;
        public TileMetadata tileMetadata;
    }
}