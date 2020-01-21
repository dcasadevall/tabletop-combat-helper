using System;
using UnityEngine;

namespace Map.MapData.TileMetadata {
    [Serializable]
    public class TileMetadataPair {
        public Vector2 tileCoords;
        public TileMetadata tileMetadata;

        public TileMetadataPair(Vector2 tileCoords) {
            this.tileCoords = tileCoords;
            this.tileMetadata = new TileMetadata();
        }
    }
}