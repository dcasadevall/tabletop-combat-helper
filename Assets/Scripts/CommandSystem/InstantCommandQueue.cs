using System.Runtime.Serialization;

namespace CommandSystem {
    /// <summary>
    /// Implementation of <see cref="ICommandQueue"/> that instantly runs the given commands.
    /// </summary>
    public class InstantCommandQueue : ICommandQueue {
        public event CommandQueued commandQueued = delegate {};

        private readonly ICommandFactory _commandFactory;

        public InstantCommandQueue(ICommandFactory commandFactory) {
            _commandFactory = commandFactory;
        }

        public void Enqueue<TData>(TData data) where TData : ISerializable {
            ICommand<TData> command = _commandFactory.Create<TData>();
            commandQueued.Invoke(command.GetType(), data);
            
            command.Run(data);
        }
    }
}