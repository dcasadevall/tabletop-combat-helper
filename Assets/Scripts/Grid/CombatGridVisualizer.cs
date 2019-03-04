using Debugging;
using UnityEngine;
using Zenject;

namespace Grid {
    /// <summary>
    /// Debug MonoBehaviour that allows os to visualize an <see cref="ICombatGrid"/>.
    /// </summary>
    public class CombatGridVisualizer : IInitializable, ITickable {
        private ICombatGrid _combatGrid;
        private IDebugSettings _debugSettings;
        
        public CombatGridVisualizer(ICombatGrid combatGrid, IDebugSettings debugSettings) {
            _combatGrid = combatGrid;
            _debugSettings = debugSettings;
        }

        public void Initialize() {
        }
        
        public void Tick() {
        }

        public class GridCellFactory : PlaceholderFactory<SpriteRenderer> {
        }
    }
}