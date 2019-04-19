
namespace Networking.Matchmaking {
    public interface IRoomSettings {
        string Name { get; }
        byte NumPlayers { get; }
        bool IsVisible { get; }
    }
}