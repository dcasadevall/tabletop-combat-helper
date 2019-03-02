using Grid.Serialized;
using Ninject;
using Ninject.Unity;
using UnityEngine;

namespace Grid {
    /// <summary>
    /// Monobehaviour used to load the selected <see cref="GridData"/> into the current
    /// <see cref="ICombatGrid"/>
    /// </summary>
    public class GridDataLoadingBehaviour : DIMono {
        public GridData gridData;
        
        [Inject]
        public ICombatGrid Grid { get; set; }

        private void Start() {
            Grid.LoadGridData(gridData);
        }
    }
}