using Grid.Serialized;
using UnityEngine;
using Zenject;

namespace Grid {
    /// <summary>
    /// <see cref="MonoBehaviour"/> used to load the selected <see cref="GridData"/> into the current
    /// <see cref="IGrid"/>.
    /// </summary>
    public class GridDataLoader : MonoBehaviour {
#pragma warning disable 649
        [SerializeField]
        private GridData _gridData;
#pragma warning restore 649
        
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