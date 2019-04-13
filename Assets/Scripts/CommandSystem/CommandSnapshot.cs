using System;
using System.Runtime.Serialization;

namespace CommandSystem {
    public class CommandSnapshot : ICommandSnapshot {
        private readonly Action _doAction;
        private readonly Action _undoAction;

        public bool IsInitialGameState { get; }
        public ISerializable Data { get; }
        public Type Type { get; }
        public TimeSpan ExecutionTime { get; }

        public CommandSnapshot(Action doAction, Action undoAction, bool isInitialGameState, ISerializable data,
                               Type type, TimeSpan executionTime) {
            _doAction = doAction;
            _undoAction = undoAction;
            Data = data;
            Type = type;
            ExecutionTime = executionTime;
            IsInitialGameState = isInitialGameState;
        }

        public void Redo() {
            _doAction.Invoke();
        }

        public void Undo() {
            _undoAction.Invoke();
        }
    }
}