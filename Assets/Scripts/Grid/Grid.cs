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

        private Rect _tileBounds = Rect.zero;
        public Rect TileBounds {
            get { return _tileBounds; }
            
        }

        public void LoadGridData(IGridData gridData) {
            _numTilesX = System.Math.Max(1, gridData.NumTilesX);
            _numTilesY = System.Math.Max(1, gridData.NumTilesY);
            _worldSpaceBounds = new Rect(-TileSize * NumTilesX / 2.0f,
                                         -TileSize * NumTilesY / 2.0f,
                                         NumTilesX * TileSize,
                                         NumTilesY * TileSize);
            _tileBounds = new Rect(Vector2.zero, 
                                   new Vector2(NumTilesX, NumTilesY));
        }
        
    }
}