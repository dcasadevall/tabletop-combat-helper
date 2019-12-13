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
    public class SerialCommandQueue : ICommandQueue, IPausableCommandQueue, ITickable {
        private readonly ICommandFactory _commandFactory;
        private readonly IClock _clock;
        private readonly ILogger _logger;
        private readonly List<ICommandQueueListener> _listeners = new List<ICommandQueueListener>();
        private readonly Queue<PendingCommand> _pendingCommands = new Queue<PendingCommand>();
        private bool _isPaused;
        private bool _processingCommand;
        private uint _nextIndex;

        public SerialCommandQueue(ICommandFactory commandFactory, IClock clock, ILogger logger) {
            _commandFactory = commandFactory;
            _clock = clock;
            _logger = logger;
        }

        public void AddListener(ICommandQueueListener listener) {
            _listeners.Add(listener);
        }

        public void Enqueue(Type commandType, Type dataType, ISerializable data,
                            CommandSource source) {
            _pendingCommands.Enqueue(new PendingCommand(commandType, dataType, data, source));
        }

        public void Tick() {
            if (_isPaused) {
                return;
            }
            
            if (_processingCommand) {
                return;
            }

            if (_pendingCommands.Count == 0) {
                return;
            }

            ProcessNextCommand();
        }

        private void ProcessNextCommand() {
            _processingCommand = true;
            PendingCommand pendingCommand = _pendingCommands.Dequeue();
            ICommand command =
                _commandFactory.Create(pendingCommand.commandType, pendingCommand.dataType, pendingCommand.data);
            if (command == null) {
                _logger.LogError(LoggedFeature.CommandSystem,
                                 "Command is not bound. Have you created a AbstractCommandsInstaller for your system?");
                return;
            }

            IObservable<Unit> observable = command.Run();
            IObserver<Unit> observer = Observer.Create<Unit>(HandleCommandSuccess, HandleCommandError);
            observable.Subscribe(observer);

            // Notify listeners
            CommandSnapshot commandSnapshot =
                new CommandSnapshot(command,
                                    pendingCommand.data,
                                    _clock.Now,
                                    pendingCommand.source);
            foreach (var commandQueueListener in _listeners) {
                commandQueueListener.HandleCommandQueued(commandSnapshot);
            }
        }

        private void HandleCommandSuccess(Unit unit) {
            _processingCommand = false;
        }

        private void HandleCommandError(Exception exception) {
            _logger.LogError(LoggedFeature.CommandSystem, "Error executing command: {0}", exception);
            _processingCommand = false;
        }

        public void Pause() {
            _logger.Log(LoggedFeature.CommandSystem, "Pausing Command Execution");
            _isPaused = true;
        }
        
        public void Resume() {
            _logger.Log(LoggedFeature.CommandSystem, "Resuming Command Execution");
            _isPaused = false;
        }

        private class PendingCommand {
            public readonly Type commandType;
            public readonly Type dataType;
            public readonly ISerializable data;
            public readonly CommandSource source;

            public PendingCommand(Type commandType, Type dataType, ISerializable data,
                                  CommandSource source) {
                this.commandType = commandType;
                this.dataType = dataType;
                this.data = data;
                this.source = source;
            }
        }
    }
}