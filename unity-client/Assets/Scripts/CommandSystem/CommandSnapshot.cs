using System;
using System.Runtime.Serialization;

namespace CommandSystem {
    public class CommandSnapshot : ICommandSnapshot {
        public ICommand Command { get; }
        public ISerializable Data { get; }
        public TimeSpan ExecutionTime { get; }
        public CommandSource Source { get; }

        public CommandSnapshot(ICommand command, ISerializable data, TimeSpan executionTime, CommandSource source) {
            Command = command;
            Data = data;
            ExecutionTime = executionTime;
            Source = source;
        }
    }
}