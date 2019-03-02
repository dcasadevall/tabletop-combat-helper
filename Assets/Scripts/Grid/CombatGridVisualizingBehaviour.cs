using Ninject;
using Ninject.Unity;

namespace Grid {
    /// <summary>
    /// Debug MonoBehaviour that allows os to visualize an <see cref="ICombatGrid"/>.
    /// </summary>
    public class CombatGridVisualizingBehaviour : DIMono {
        [Inject]
        private ICombatGrid CombatGrid { get; set; }
    }
}