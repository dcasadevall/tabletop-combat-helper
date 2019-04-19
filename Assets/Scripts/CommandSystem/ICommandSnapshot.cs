using System;
using System.Runtime.Serialization;

namespace CommandSystem {
    /// <summary>
    /// A stateful version of a command that can be executed / reverted at will.
    /// </summary>
    public interface ICommandSnapshot {
        /// <summary>
        /// The serializable data used to run the command.
        /// </summary>
        ISerializable Data { get; }
        /// <summary>
        /// The command executed in this snapshot.
        /// </summary>
        ICommand Command { get; }
        /// <summary>
        /// Time (from game start) at which this command was first executed.
        /// Note that calls to <see cref="ICommand.Run"/> do not alter this time.
        /// </summary>
        TimeSpan ExecutionTime { get; }
        /// <summary>
        /// The originator of this command. Useful for avoiding queuing conflicts.
        /// I.e: one actor queues a command in response to another command, but does not want to queue a command
        /// in response to its own commands.
        /// </summary>
        CommandSource Source { get; }
    }
}