using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;

namespace Grid.Brushes {
    [CustomEditor(typeof(PathBrush))]
    public class PathBrushEditor : GridBrushEditor
    {
        public override void OnPaintSceneGUI(GridLayout gridLayout,
                                             GameObject brushTarget,
                                             BoundsInt position,
                                             GridBrushBase.Tool tool,
                                             bool executing) {
            base.OnPaintSceneGUI(gridLayout, brushTarget, position, tool, executing);
        }
    }
}
