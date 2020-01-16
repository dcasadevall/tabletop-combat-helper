using System;
using CameraSystem;
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
    public class FogOfWarMeshFilterColorSetter : ITileVisibilityDelegate, IInitializable, IDisposable {
        private static Color32 fogVertexColor = new Color32(0, 0, 0, 0);
        private static Color32 outOfViewVertexColor = new Color32(0, 0, 0, 130);
        private static Color32 inViewVertexColor = new Color32(0, 0, 0, 255);
        
        private readonly MeshFilter _meshFilter;
        private readonly IGrid _grid;
        private readonly IGridMeshGenerator _gridMeshGenerator;

        private bool _pendingColorUpdates;
        private Color32[] _colors;
        private IDisposable _observer;

        public FogOfWarMeshFilterColorSetter(MeshFilter meshFilter,
                                             IGrid grid,
                                             IGridMeshGenerator gridMeshGenerator) {
            _meshFilter = meshFilter;
            _grid = grid;
            _gridMeshGenerator = gridMeshGenerator;
            // For a row of N tiles, there are N + 1 vertices.
            _colors = new Color32[(_grid.NumTilesX + 1) * (_grid.NumTilesY + 1)];
        }

        public void Initialize() {
            // Create the mesh which will show the fog. It is anchored bottom-left (grid origin).
            _meshFilter.mesh = _gridMeshGenerator.Generate(_grid);
            _meshFilter.transform.position = new Vector3(_grid.WorldSpaceBounds().min.x,
                                                         _grid.WorldSpaceBounds().min.y,
                                                         DepthConstants.FOG_OF_WAR_DEPTH);
            
            // Tick every 5 frames
            _observer = Observable.EveryUpdate()
                                  .BatchFrame(10, FrameCountType.Update)
                                  .Where(_ => _pendingColorUpdates)
                                  .TakeWhile(_ => _observer != null)
                                  .Subscribe(x => Tick());
        }

        public void Dispose() {
            _observer.Dispose();
            _observer = null;
        }

        private void Tick() {
            _meshFilter.mesh.colors32 = _colors;
            _pendingColorUpdates = false;
        }
        
        public void HandleTileVisibilityChanged(IntVector2 tileCoords, TileVisibilityType tileVisibilityType) {
            Color32 selectedColor;
            if (tileVisibilityType == TileVisibilityType.NotVisited) {
                selectedColor = fogVertexColor;
            } else if (tileVisibilityType == TileVisibilityType.VisitedNotInSight) {
                selectedColor = outOfViewVertexColor;
            } else {
                selectedColor = inViewVertexColor;
            }

            // add 1 to map size to account for N+1 vertices in the tiles
            int tileVertexIndex = tileCoords.y * ((int) _grid.NumTilesX + 1) +
                                  tileCoords.x;

            // Assign the color
            _colors[tileVertexIndex] = selectedColor;
            _colors[tileVertexIndex + 1] = selectedColor;
            _colors[tileVertexIndex + _grid.NumTilesX + 1] = selectedColor;
            _colors[tileVertexIndex + _grid.NumTilesX + 2] = selectedColor;
            
            // Mark pending color updates
            _pendingColorUpdates = true;
        }
    }
}