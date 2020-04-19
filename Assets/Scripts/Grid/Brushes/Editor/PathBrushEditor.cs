using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

namespace Grid.Brushes {
    [CustomEditor(typeof(PathBrush))]
    public class PathBrushEditor : GridBrushEditor
    {
        private PathBrush Brush {
            get {
                return target as PathBrush;
            }
        }
        public override void OnPaintSceneGUI(GridLayout gridLayout,
                                             GameObject brushTarget,
                                             BoundsInt position,
                                             GridBrushBase.Tool tool,
                                             bool executing) {
            base.OnPaintSceneGUI(gridLayout, brushTarget, position, tool, executing);

            if (Brush.isDrawing == false) {
                return;
            }
            
            foreach (Vector3Int pos in Brush.AllowedNextPositions) {
                Vector3 worldPosition = gridLayout.CellToWorld(pos);
                DrawBox(worldPosition, gridLayout.cellSize);
            }
        }

        private void DrawBox(Vector3 position, Vector3 size) {
            var min = position;
            var max = position + size;
            
            GL.PushMatrix();
            GL.MultMatrix(GUI.matrix);
            GL.Begin(GL.LINES);
            Handles.color = Color.green;
            Handles.DrawLine(new Vector3(min.x, min.y, min.z), new Vector3(max.x, min.y, min.z));
            Handles.DrawLine(new Vector3(max.x, min.y, min.z), new Vector3(max.x, max.y, min.z));
            Handles.DrawLine(new Vector3(max.x, max.y, min.z), new Vector3(min.x, max.y, min.z));
            Handles.DrawLine(new Vector3(min.x, max.y, min.z), new Vector3(min.x, min.y, min.z));
            GL.End();
            GL.PopMatrix();
        }
    }
}
