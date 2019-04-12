using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Logging;

namespace CommandSystem {
    /// <summary>
    /// Implementation of <see cref="ICommandQueue"/> that instantly runs the given commands.
    /// </summary>
    public class InstantCommandQueue : ICommandQueue {
        private readonly ICommandFactory _commandFactory;
        private readonly ILogger _logger;
        private readonly List<ICommandQueueListener> _listeners = new List<ICommandQueueListener>();

        public InstantCommandQueue(ICommandFactory commandFactory, ILogger logger) {
            _commandFactory = commandFactory;
            _logger = logger;
        }

        public void AddListener(ICommandQueueListener listener) {
            _listeners.Add(listener);
        }

        public void Enqueue<TData>(TData data) where TData : ISerializable {
            ICommand<TData> command = _commandFactory.Create<TData>();
            if (command == null) {
                _logger.LogError(LoggedFeature.CommandSystem,
                                 "Command is not bound. Have you created an AbstractCommandsInstaller for your system?");
                return;
            }
            
            foreach (var commandQueueListener in _listeners) {
                commandQueueListener.HandleCommandQueued(data);
            }
            command.Run(data);
        }
    }
}