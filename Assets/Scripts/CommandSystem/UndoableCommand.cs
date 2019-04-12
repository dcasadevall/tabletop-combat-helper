using System.Runtime.Serialization;

namespace CommandSystem {
    public class UndoableCommand<TData> : IUndoable where TData : ISerializable {
        private readonly TData _data;
        private readonly ICommand<TData> _command;

        public UndoableCommand(ICommand<TData> command, TData data) {
            _command = command;
            _data = data;
        }
        
        public void Undo() {
            _command.Undo(_data);
        }
    }
}