using System;
using System.Runtime.Serialization;

namespace CommandSystem {
    public class CommandSnapshot : ICommandSnapshot {
        private readonly Action _doAction;
        private readonly Action _undoAction;
        private readonly ISerializable _data;
        private readonly TimeSpan _executionTime;

        public ISerializable Data {
            get {
                return _data;
            }
        }

        public TimeSpan ExecutionTime {
            get {
                return _executionTime;
            }
        }

        public CommandSnapshot(Action doAction, Action undoAction, ISerializable data, TimeSpan executionTime) {
            _doAction = doAction;
            _undoAction = undoAction;
            _data = data;
            _executionTime = executionTime;
        }

        public void Redo() {
            _doAction.Invoke();
        }

        public void Undo() {
            _undoAction.Invoke();
        }
    }
}