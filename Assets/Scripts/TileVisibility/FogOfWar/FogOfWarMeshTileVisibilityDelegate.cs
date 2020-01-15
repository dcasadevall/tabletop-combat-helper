using System;
using System.Collections.Generic;
using Grid;
using Math;
using UniRx;
using UnityEngine;
using Utils.Mesh;
using Zenject;

namespace TileVisibility.FogOfWar {
    /// <summary>
    /// Implementation of <see cref="ITileVisibilityDelegate"/> which will display a fog of war using a mesh,
    /// based on tile visibility updates.
    /// It batches updates to avoid updating the mesh too often.
    /// </summary>
    public class FogOfWarMeshTileVisibilityDelegate : ITileVisibilityDelegate, IInitializable, IDisposable {
        private readonly MeshFilter _meshFilter;
        private readonly FogOfWarMeshFilterColorSetter _colorSetter;
        private readonly IGrid _grid;
        private readonly IGridMeshGenerator _gridMeshGenerator;
        
        private IDisposable _observer;
        private Dictionary<IntVector2, TileVisibilityType> _batchedUpdates = new Dictionary<IntVector2, TileVisibilityType>();

        public FogOfWarMeshTileVisibilityDelegate(MeshFilter meshFilter,
                                                  FogOfWarMeshFilterColorSetter colorSetter,
                                                  IGrid grid, 
                                                  IGridMeshGenerator gridMeshGenerator) {
            _meshFilter = meshFilter;
            _colorSetter = colorSetter;
            _grid = grid;
            _gridMeshGenerator = gridMeshGenerator;
        }

        public void Initialize() {
            // Create the mesh which will show the fog. It is anchored bottom-left (grid origin).
            _meshFilter.mesh = _gridMeshGenerator.Generate(_grid);
            _meshFilter.transform.position = new Vector3(_grid.WorldSpaceBounds().min.x,
                                                         _grid.WorldSpaceBounds().min.y,
                                                         -3.0f);
            
            // Tick every 5 frames
            _observer = Observable.EveryUpdate()
                                  .BatchFrame(10, FrameCountType.Update)
                                  .TakeWhile(_ => _observer != null)
                                  .Subscribe(x => Tick());
        }

        public void Dispose() {
            _observer.Dispose();
            _observer = null;
        }

        private void Tick() {
            foreach (var update in _batchedUpdates) {
                
            }
            _batchedUpdates.Clear();
        }
        
        public void HandleTileVisibilityChanged(IntVector2 tileCoords, TileVisibilityType tileVisibilityType) {
            _batchedUpdates[tileCoords] = tileVisibilityType;
        }
    }
}