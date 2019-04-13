using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Logging;
using Map.Commands;
using Util;

namespace CommandSystem {
    /// <summary>
    /// Implementation of <see cref="ICommandQueue"/> that instantly runs the given commands.
    /// </summary>
    public class InstantCommandQueue : ICommandQueue {
        private readonly ICommandFactory _commandFactory;
        private readonly IClock _clock;
        private readonly ILogger _logger;
        private readonly List<ICommandQueueListener> _listeners = new List<ICommandQueueListener>();

        public InstantCommandQueue(ICommandFactory commandFactory, IClock clock, ILogger logger) {
            _commandFactory = commandFactory;
            _clock = clock;
            _logger = logger;
        }

        public void AddListener(ICommandQueueListener listener) {
            _listeners.Add(listener);
        }

        public void Enqueue(Type commandType, ISerializable data) {
            ICommand<ISerializable> command = _commandFactory.Create(data.GetType());
            if (command == null) {
                _logger.LogError(LoggedFeature.CommandSystem,
                                 "Command is not bound. Have you created an AbstractCommandsInstaller for your system?");
                return;
            }

            // Execute the command.
            command.Run(data);

            // Notify listeners.
            CommandSnapshot commandSnapshot =
                new CommandSnapshot(() => command.Run(data),
                                    () => command.Undo(data),
                                    command.IsInitialGameStateCommand,
                                    data,
                                    command.GetType(),
                                    _clock.Now);
            
            foreach (var commandQueueListener in _listeners) {
                commandQueueListener.HandleCommandQueued(commandSnapshot);
            }
        }

        public void Enqueue<TData>(TData data) where TData : ISerializable {
            // Create the command.
            ICommand<TData> command = _commandFactory.Create<TData>();
            if (command == null) {
                _logger.LogError(LoggedFeature.CommandSystem,
                                 "Command is not bound. Have you created an AbstractCommandsInstaller for your system?");
                return;
            }

            // Execute the command.
            command.Run(data);

            // Notify listeners.
            CommandSnapshot commandSnapshot =
                new CommandSnapshot(() => command.Run(data),
                                    () => command.Undo(data),
                                    command.IsInitialGameStateCommand,
                                    data,
                                    command.GetType(),
                                    _clock.Now);
            
            foreach (var commandQueueListener in _listeners) {
                commandQueueListener.HandleCommandQueued(commandSnapshot);
            }
        }
    }
}