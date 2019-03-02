
namespace Units {
    /// <summary>
    /// A Unit represents a player or npc in the tabletop game.
    /// It can be position in the <see cref="Grid.ICombatGrid"/> and interacted with.
    /// </summary>
    public interface IUnit {
        UnitId UnitId { get; }
    }
}