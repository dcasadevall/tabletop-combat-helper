using Grid.Serialized;
using UnityEngine;

namespace Grid {
    /// <summary>
    /// <see cref="IGrid"/> implementation that simply stores the values given by <see cref="LoadGridData"/>.
    /// It also provides a set of default values.
    /// </summary>
    internal class Grid : IGrid {
        private uint _numTilesX = 80;
        public uint NumTilesX {
            get {
                return _numTilesX;
            }
        }

        private uint _numTilesY = 80;
        public uint NumTilesY {
            get {
                return _numTilesY;
            }
        }

        private uint _tileSize = 1;
        public uint TileSize {
            get {
                return _tileSize;
            }
        }

        private Vector2 _originWorldPosition;
        public Vector2 OriginWorldPosition {
            get {
                return _originWorldPosition;
            }
        }

        public void LoadGridData(IGridData gridData) {
            _numTilesX = System.Math.Max(1, gridData.NumTilesX);
            _numTilesY = System.Math.Max(1, gridData.NumTilesY);
            _originWorldPosition = gridData.OriginWorldPosition ??
                                   new Vector2(-_numTilesX * TileSize / 2.0f, -_numTilesY * TileSize / 2.0f);
        }
    }
}