using System;
using System.Runtime.Serialization;

namespace CommandSystem {
    public class CommandSnapshot : ICommandSnapshot {
        public ICommand Command { get; }
        public ISerializable Data { get; }
        public TimeSpan ExecutionTime { get; }

        public CommandSnapshot(ICommand command, ISerializable data, TimeSpan executionTime) {
            Command = command;
            Data = data;
            ExecutionTime = executionTime;
        }
    }
}