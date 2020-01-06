using UnityEngine;

namespace CameraSystem {
    public static class CameraRectUtils {
        public static Rect ViewPortRectToWorldRect(Camera camera, Rect viewPortRect) {
            Vector3 bottomLeft = camera.ViewportToWorldPoint(new Vector2(viewPortRect.xMin, viewPortRect.yMin));
            Vector3 topRight = camera.ViewportToWorldPoint(new Vector2(viewPortRect.xMax, viewPortRect.yMax));
            return Rect.MinMaxRect(bottomLeft.x, bottomLeft.y, topRight.x, topRight.y);
        }
    }
}