using System;
using Grid.Serialized;
using UnityEngine;

namespace Grid {
    /// <summary>
    /// <see cref="ICombatGrid"/> implementation that simply stores the values given by <see cref="LoadGridData"/>.
    /// It also provides a set of default values.
    /// </summary>
    public class CombatGrid : ICombatGrid {
        private uint _numTilesX = 400;
        public uint NumTilesX {
            get {
                return _numTilesX;
            }
        }

        private uint _numTilesY = 400;
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

        private Vector2 _worldSpaceOrigin = Vector2.zero;
        public Vector2 WorldSpaceOrigin {
            get {
                return _worldSpaceOrigin;
            }
        }

        public void LoadGridData(GridData gridData) {
            _numTilesX = Math.Max(1, gridData.numTilesX);
            _numTilesY = Math.Max(1, gridData.numTilesY);
        }
    }
}