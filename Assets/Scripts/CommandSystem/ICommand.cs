using System.Runtime.Serialization;

namespace CommandSystem {
    public interface ICommand {
        /// <summary>
        /// Scene setup commands are those which can be used to setup an initial game state.
        /// Things like "loading a map" "spawning player units", etc.. that should not be Undone and
        /// can be loaded atomically on start.
        /// </summary>
        bool IsInitialGameStateCommand { get; }
        
        void Run(ISerializable data);
        void Undo(ISerializable data);
    }
}