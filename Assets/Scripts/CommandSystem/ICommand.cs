using System.Runtime.Serialization;

namespace CommandSystem {
    public interface ICommand<in TData> where TData : ISerializable {
        /// <summary>
        /// Scene setup commands are those which can be used to setup an initial game state.
        /// Things like "loading a map" "spawning player units", etc.. that should not be Undone and
        /// can be loaded atomically on start.
        /// </summary>
        bool IsInitialGameStateCommand { get; }
        
        void Run(TData data);
        void Undo(TData data);
    }

    public interface ICommand : ICommand<ISerializable> {
    }
}