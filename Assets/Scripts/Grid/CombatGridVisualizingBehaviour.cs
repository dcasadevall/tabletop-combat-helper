using UnityEngine;
using Zenject;

namespace Grid {
    /// <summary>
    /// Debug MonoBehaviour that allows os to visualize an <see cref="ICombatGrid"/>.
    /// </summary>
    public class CombatGridVisualizingBehaviour : MonoBehaviour {
        private ICombatGrid _combatGrid;
        
        [Inject]
        public void Construct(ICombatGrid combatGrid) {
            _combatGrid = combatGrid;
        }
    }
}