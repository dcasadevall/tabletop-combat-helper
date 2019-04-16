using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Logging;
using Map.Commands;
using UniRx;
using Util;
using Zenject;

namespace CommandSystem {
    /// <summary>
    /// Implementation of <see cref="ICommandQueue"/> that serially executes the given commands,
    /// adding them to a processing queue as they are received.
    /// </summary>
    public class SerialCommandQueue : ICommandQueue, ITickable {
        private readonly ICommandFactory _commandFactory;
        private readonly IClock _clock;
        private readonly ILogger _logger;
        private readonly List<ICommandQueueListener> _listeners = new List<ICommandQueueListener>();
        private readonly Queue<ICommand> _processingQueue = new Queue<ICommand>();
        private bool _processingCommand;

        public SerialCommandQueue(ICommandFactory commandFactory, IClock clock, ILogger logger) {
            _commandFactory = commandFactory;
            _clock = clock;
            _logger = logger;
        }

        public void AddListener(ICommandQueueListener listener) {
            _listeners.Add(listener);
        }

        public void Enqueue(Type commandType, Type dataType, ISerializable data) {
            ICommand command = _commandFactory.Create(commandType, dataType, data);
            if (command == null) {
                _logger.LogError(LoggedFeature.CommandSystem,
                                 "Command is not bound. Have you created an AbstractCommandsInstaller for your system?");
                return;
            }

            // Enqueue the command and notify listeners.
            _processingQueue.Enqueue(command);
            CommandSnapshot commandSnapshot = new CommandSnapshot(command, data, _clock.Now);
            foreach (var commandQueueListener in _listeners) {
                commandQueueListener.HandleCommandQueued(commandSnapshot);
            }
        }

        public void Tick() {
            if (_processingCommand) {
                return;
            }

            if (_processingQueue.Count == 0) {
                return;
            }

            _processingCommand = true;
            ICommand command = _processingQueue.Dequeue();
            IObservable<Unit> observable = command.Run();
            IObserver<Unit> observer = Observer.Create<Unit>(HandleCommandSuccess, HandleCommandError);
            observable.Subscribe(observer);
        }

        private void HandleCommandSuccess(Unit unit) {
            _processingCommand = false;
        }
        
        private void HandleCommandError(Exception exception) {
            _logger.LogError(LoggedFeature.CommandSystem, "Error executing command: {0}", exception);
            _processingCommand = false;
        }
    }
}