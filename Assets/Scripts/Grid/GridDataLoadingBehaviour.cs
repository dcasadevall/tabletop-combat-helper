using Grid.Serialized;
using UnityEngine;
using Zenject;

namespace Grid {
    /// <summary>
    /// <see cref="MonoBehaviour"/> used to load the selected <see cref="GridData"/> into the current
    /// <see cref="IGrid"/>.
    /// </summary>
    public class GridDataLoadingBehaviour : MonoBehaviour {
        [SerializeField]
        private GridData _gridData;
        private IGrid _grid;
        
        [Inject]
        public void Construct(IGrid grid) {
            _grid = grid;
        }

        private void Start() {
            _grid.LoadGridData(_gridData);
        }
    }
}