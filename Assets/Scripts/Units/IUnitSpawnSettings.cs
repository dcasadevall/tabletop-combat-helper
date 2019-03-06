
namespace Units {
    public interface IUnitSpawnSettings {
        /// <summary>
        /// Units that can be spawned after map is loaded and players are placed.
        /// These units also include pets that player units can have.
        /// </summary>
        IUnit[] NonPlayerUnits { get; }
        
        /// <summary>
        /// Player units are found here. This does not include pets, which can be found in
        /// <see cref="NonPlayerUnits"/>
        /// </summary>
        IUnit[] PlayerUnits { get; }
    }
}