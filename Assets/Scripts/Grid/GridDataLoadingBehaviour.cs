using Grid.Serialized;
using UnityEngine;
using Zenject;

namespace Grid {
    /// <summary>
    /// <see cref="MonoBehaviour"/> used to load the selected <see cref="GridData"/> into the current
    /// <see cref="ICombatGrid"/>
    /// </summary>
    public class GridDataLoadingBehaviour : MonoBehaviour {
        [SerializeField]
        private GridData _gridData;
        private ICombatGrid _combatGrid;
        
        [Inject]
        public void Construct(ICombatGrid combatGrid) {
            _combatGrid = combatGrid;
        }

        private void Start() {
            _combatGrid.LoadGridData(_gridData);
        }
    }
}