using Grid.Serialized;
using UnityEngine;
using Zenject;

namespace Grid {
    /// <summary>
    /// <see cref="MonoBehaviour"/> used to load the selected <see cref="GridData"/> into the current
    /// <see cref="IGrid"/>.
    /// </summary>
    public class GridDataLoader : IInitializable {
        private IGrid _grid;
        private IGridData _gridData;

        public GridDataLoader(IGrid grid, IGridData gridData) {
            _grid = grid;
            _gridData = gridData;
        }

        public void Initialize() {
            _grid.LoadGridData(_gridData);
        }
    }
}