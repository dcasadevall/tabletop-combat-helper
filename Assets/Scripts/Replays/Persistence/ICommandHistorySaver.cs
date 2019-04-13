namespace Replays.Persistence {
    public interface ICommandHistorySaver {
        CommandHistorySaveInfo SaveCommandHistory(string name);
    }
}