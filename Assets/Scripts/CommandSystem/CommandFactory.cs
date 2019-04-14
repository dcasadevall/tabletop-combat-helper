using System;
using System.Collections.Generic;
using Logging;
using Zenject;
using ILogger = Logging.ILogger;

namespace CommandSystem {
    public class CommandFactory : ICommandFactory {
        private readonly ILogger _logger;
        private readonly HashSet<DiContainer> _containers = new HashSet<DiContainer>();

        public CommandFactory(DiContainer container, ILogger logger) {
            _logger = logger;
            _containers.Add(container);
        }

        public void RegisterSceneContainer(DiContainer container) {
            _containers.Add(container);
        }

        public ICommand Create<TCommand>() where TCommand : class, ICommand {
            foreach (var diContainer in _containers) {
                TCommand command = diContainer.TryResolve<TCommand>();
                if (command != null) {
                    return command;
                }
            }

            _logger.LogError(LoggedFeature.CommandSystem,
                             "Command not found in registered contexts: {0}",
                             typeof(ICommand));
            return null;
        }

        public ICommand Create(Type type) {
            foreach (var diContainer in _containers) {
                ICommand command = (ICommand)diContainer.TryResolve(type);
                if (command != null) {
                    return command;
                }
            }

            _logger.LogError(LoggedFeature.CommandSystem,
                             "Command not found in registered contexts: {0}",
                             typeof(ICommand));
            return null;
        }
    }
}