
using Grid;

namespace Units {
    /// <summary>
    /// A Unit represents a player or npc in the tabletop game.
    /// It can be position in the <see cref="IGrid"/> and interacted with.
    /// </summary>
    public interface IUnit {
        UnitId UnitId { get; }
    }
}