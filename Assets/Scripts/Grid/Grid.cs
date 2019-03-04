using System;
using Grid.Serialized;
using UnityEngine;

namespace Grid {
    /// <summary>
    /// <see cref="IGrid"/> implementation that simply stores the values given by <see cref="LoadGridData"/>.
    /// It also provides a set of default values.
    /// </summary>
    public class Grid : IGrid {
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

        private Rect _worldSpaceBounds = Rect.zero;
        public Rect WorldSpaceBounds {
            get {
                return _worldSpaceBounds;
            }
        }

        public void LoadGridData(GridData gridData) {
            _numTilesX = Math.Max(1, gridData.numTilesX);
            _numTilesY = Math.Max(1, gridData.numTilesY);
            _worldSpaceBounds = new Rect(-TileSize * NumTilesX / 2.0f,
                                         -TileSize * NumTilesY / 2.0f,
                                         NumTilesX * TileSize,
                                         NumTilesY * TileSize);
        }
    }
}