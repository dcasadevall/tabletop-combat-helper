using System.Runtime.Serialization;

namespace CommandSystem {
    public class GenericCommandAdapter : ICommand {
        private readonly ICommand<ISerializable> _command;
        
        public bool IsInitialGameStateCommand {
            get {
                return _command.IsInitialGameStateCommand;
            }
        }

        public GenericCommandAdapter(ICommand<ISerializable> command) {
            _command = command;
        }
        
        public void Run(ISerializable data) {
            _command.Run(data);
        }

        public void Undo(ISerializable data) {
            _command.Undo(data);
        }
    }
}