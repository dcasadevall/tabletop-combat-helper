using System;
using System.Runtime.Serialization;
using Logging;

namespace CommandSystem {
    /// <summary>
    /// Implementation of <see cref="ICommandQueue"/> that instantly runs the given commands.
    /// </summary>
    public class InstantCommandQueue : ICommandQueue {
        public event CommandQueued commandQueued = delegate {};
        
        private readonly ICommandFactory _commandFactory;
        private readonly ILogger _logger;

        public InstantCommandQueue(ICommandFactory commandFactory, ILogger logger) {
            _commandFactory = commandFactory;
            _logger = logger;
        }

        public void Enqueue<TCommand, TData>(TData data) where TCommand : ICommand<TData> where TData : ISerializable {
            ICommand<ISerializable> command = _commandFactory.Create(typeof(TCommand));
            if (command == null) {
                _logger.LogError(LoggedFeature.CommandSystem,
                                 "Command is not bound. Have you created an AbstractCommandsInstaller for your system?");
                return;
            }
            
            commandQueued.Invoke(command, data);
            command.Run(data);
        }
    }
}