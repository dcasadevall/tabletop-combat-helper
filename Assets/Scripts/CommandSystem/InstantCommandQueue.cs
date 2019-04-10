using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CommandSystem {
    /// <summary>
    /// Implementation of <see cref="ICommandQueue"/> that instantly runs the given commands.
    /// </summary>
    public class InstantCommandQueue : ICommandQueue {
        private readonly ICommandFactory _commandFactory;
        private Queue<ISerializable> _executedCommands = new Queue<ISerializable>();

        public InstantCommandQueue(ICommandFactory commandFactory) {
            _commandFactory = commandFactory;
        }

        public void Enqueue<TData>(TData data) where TData : ISerializable {
            _commandFactory.Create<TData>().Run(data);
            _executedCommands.Enqueue(data);
        }
    }
}