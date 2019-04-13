using System;
using System.Runtime.Serialization;

namespace CommandSystem {
    /// <summary>
    /// A stateful version of a command that can be executed / reverted at will.
    /// </summary>
    public interface ICommandSnapshot {
        /// <summary>
        /// If true, the command executed cannot be undone, and is part of the initial game state.
        /// </summary>
        bool IsInitialGameState { get; }
        /// <summary>
        /// The serializable data used to run the command.
        /// </summary>
        ISerializable Data { get; }
        /// <summary>
        /// The type of this command. Useful for serialization.
        /// </summary>
        Type Type { get; }
        /// <summary>
        /// Time (from game start) at which this command was first executed.
        /// Note that calls to <see cref="Redo"/> do not alter this time.
        /// </summary>
        TimeSpan ExecutionTime { get; }
        
        /// <summary>
        /// Replays this command. This does not reset "TimeSpan".
        /// </summary>
        void Redo();
        /// <summary>
        /// Returns to the state of the game prior to the command's execution.
        /// Note that this requires the command to properly implement Undo()
        /// </summary>
        void Undo();
    }
}