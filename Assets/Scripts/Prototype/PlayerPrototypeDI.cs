using Ninject;
using Ninject.Unity;
using Units.Serialized;

namespace Prototype {
    /// <summary>
    /// This class is dumb, and the fact that we need it is also dumb.
    /// <see cref="PlayerPrototype"/> needs to inherit from <see cref="UnityEngine.Networking.NetworkBehaviour"/>,
    /// so it cannot also inherit from <see cref="DIMono"/>.
    /// </summary>
    public class PlayerPrototypeDI : DIMono {
        [Inject] 
        public IUnitData[] unitDatas { get; private set; }
    }
}