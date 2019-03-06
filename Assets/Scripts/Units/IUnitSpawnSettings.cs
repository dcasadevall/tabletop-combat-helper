
using Units.Serialized;

namespace Units {
    public interface IUnitSpawnSettings {
        /// <summary>
        /// Units that can be spawned after map is loaded and players are placed.
        /// These units also include pets that player units can have.
        /// </summary>
        IUnitData[] NonPlayerUnits { get; }

        /// <summary>
        /// Player units are found here. This does not include pets, which can be found in
        /// <see cref="NonPlayerUnits"/>
        /// </summary>
        IUnitData[] PlayerUnits { get; }

        /// <summary>
        /// How far initial units can spawn from the center at most.
        /// </summary>
        int MaxInitialUnitSpawnDistanceToCenter { get; }
    }
}