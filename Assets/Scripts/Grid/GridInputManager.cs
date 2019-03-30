using Grid.Positioning;
using Math;
using UnityEngine;

namespace Grid {
  public class GridInputManager : IGridInputManager {
    private readonly Camera _camera;
    private readonly IGridPositionCalculator _gridPositionCalculator;

    public GridInputManager(Camera camera, IGridPositionCalculator gridPositionCalculator) {
      _camera = camera;
      _gridPositionCalculator = gridPositionCalculator;
    }
    
    public IntVector2? GetTileAtMousePosition() {
      Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
      Vector3 curPosition = _camera.ScreenToWorldPoint(curScreenPoint);
      return _gridPositionCalculator.GetTileContainingWorldPosition(curPosition);
    }
  }
}