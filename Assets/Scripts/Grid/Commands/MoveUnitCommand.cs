using CommandSystem;

namespace Grid.Commands {
    /// <summary>
    /// Command used to place a unit on the grid, or move it from one tile to another.
    /// </summary>
    public class MoveUnitCommand : ICommand<MoveUnitData> {
        public void Run(MoveUnitData data) {
        }
    }
}