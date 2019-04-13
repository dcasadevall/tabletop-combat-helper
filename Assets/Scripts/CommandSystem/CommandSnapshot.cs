using System;
using System.Runtime.Serialization;

namespace CommandSystem {
    public class CommandSnapshot : ICommandSnapshot {
        private readonly Action _doAction;
        private readonly Action _undoAction;

        public bool IsInitialGameState { get; }
        public ISerializable Data { get; }
        public TimeSpan ExecutionTime { get; }

        public CommandSnapshot(Action doAction, Action undoAction, bool isInitialGameState, ISerializable data, TimeSpan executionTime) {
            _doAction = doAction;
            _undoAction = undoAction;
            Data = data;
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